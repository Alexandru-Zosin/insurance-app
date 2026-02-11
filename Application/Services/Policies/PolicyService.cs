using Domain.Policies;
using Application.Common;
using Application.Exceptions;
using Application.Repositories.SearchCriteria;
using Application.Services.Policies.DTOs;
using Application.Services.Shared.DTOs.BrokerDTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Application.Repositories;

namespace Application.Services.Policies;

public sealed class PolicyService(
    IPolicyRepository policyRepository,
    IClientRepository clientRepository,
    IBuildingRepository buildingRepository,
    IBrokerRepository brokerRepository,
    IPolicyDraftPrerequisitesLoader draftPrerequisitesLoader,
    IPolicyPricingService pricingService,
    IUnitOfWork uow) : IPolicyService
{
    public async Task<Result<CreateDraftPolicyResponse>> CreateDraftPolicyAsync(
        CreateDraftPolicyRequest request,
        CancellationToken ct = default)
    {
        var draftPrereqResult = await draftPrerequisitesLoader.LoadAsync(request, ct);
        if (!draftPrereqResult.IsSuccess)
            return Result<CreateDraftPolicyResponse>.Fail(draftPrereqResult.ErrorType, draftPrereqResult.ErrorMessage);

        var draftPrerequisites = draftPrereqResult.Value!;

        var draftBasePremium = request.Policy.BasePremium.MapToDomain();
        var draftTenure = request.Policy.Tenure.MapToDomain();
        var finalPremium = await pricingService.CalculateDraftFinalPremiumAsync(
            draftPrerequisites,
            request,
            DateOnly.FromDateTime(DateTime.UtcNow),
            ct);

        var newDraft = Policy.CreateDraft(
            draftPrerequisites.Client.Id,
            draftPrerequisites.Building.Id,
            draftPrerequisites.Broker.Id,
            draftTenure,
            draftBasePremium,
            draftPrerequisites.Currency.Code,
            finalPremium,
            DateOnly.FromDateTime(DateTime.UtcNow));

        policyRepository.Add(newDraft);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DuplicateKeyException)
        {
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.Conflict, "Duplicate policy number");
        }

        var response = new CreateDraftPolicyResponse(PolicyListItemDto.From(newDraft));
        return Result<CreateDraftPolicyResponse>.Ok(response);
    }

    public async Task<Result<ActivatePolicyResponse>> ActivatePolicyAsync(
        Guid policyNumber,
        CancellationToken ct = default)
    {
        var policy = await policyRepository.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<ActivatePolicyResponse>.Fail(ErrorType.NotFound, "Policy not found");

        policy.Activate();

        await policyRepository.UpdateAsync(policy, ct);
        await uow.SaveChangesAsync(ct);

        var response = new ActivatePolicyResponse(PolicyListItemDto.From(policy));
        return Result<ActivatePolicyResponse>.Ok(response);
    }

    public async Task<Result<CancelPolicyResponse>> CancelPolicyAsync(
        Guid policyNumber,
        CancelPolicyRequest request,
        CancellationToken ct = default)
    {
        var policy = await policyRepository.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<CancelPolicyResponse>.Fail(ErrorType.NotFound, "Policy not found");

        policy.Cancel(request.Reason, request.CancellationEffectiveDate);

        await policyRepository.UpdateAsync(policy, ct);
        await uow.SaveChangesAsync(ct);

        var response = new CancelPolicyResponse(PolicyListItemDto.From(policy));
        return Result<CancelPolicyResponse>.Ok(response);
    }

    public async Task<Result<GetPolicyDetailsResponse>> GetPolicyDetailsAsync(
        Guid policyNumber,
        CancellationToken ct = default)
    {
        var policy = await policyRepository.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<GetPolicyDetailsResponse>.Fail(ErrorType.NotFound, "Policy not found");

        var policyClient = await clientRepository.GetByIdAsync(policy.ClientId, ct);
        var policyBuilding = await buildingRepository.GetByIdAsync(policy.BuildingId, ct);
        var policyBroker = await brokerRepository.GetByIdAsync(policy.BrokerId, ct);

        var response = new GetPolicyDetailsResponse(PolicyDetailedDto.From(
            policy,
            ClientListItemDto.From(policyClient!),
            BuildingListItemDto.From(policyBuilding!),
            BrokerListItemDto.From(policyBroker!)));

        return Result<GetPolicyDetailsResponse>.Ok(response);
    }

    public async Task<Result<ListPoliciesResponse>> ListPoliciesAsync(
        ListPoliciesRequest request,
        CancellationToken ct = default)
    {
        var criteria = new PolicySearchCriteria(
            request.ClientId,
            request.BrokerId,
            null,
            request.Status,
            request.StartDate,
            request.EndDate);

        var matchedPolicies = await policyRepository.ListAsync(criteria, request.Page, ct);

        return Result<ListPoliciesResponse>.Ok(
            new ListPoliciesResponse(matchedPolicies.Select(PolicyListItemDto.From).ToArray()));
    }
}

