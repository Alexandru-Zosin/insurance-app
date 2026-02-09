using Application.Common;
using Application.Repositories;
using Application.Services.Policies.DTOs;
using Application.Services.Shared.DTOs.BrokerDTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Policies;
using Domain.Services;

namespace Application.Services.Policies;

public sealed class PolicyService(
    IPolicyRepository _policies,
    IClientRepository _clients,
    IBuildingRepository _buildings,
    IBrokerRepository _brokers,
    ICurrencyRepository _currencies,
    IPremiumRuleQuery _premiumRules,
    ICityRepository _cities,
    ICountyRepository _counties,
    ICountryRepository _countries,
    IPremiumCalculatorService _premiumCalculatorService,
    IUnitOfWork _uow) : IPolicyService
{
    public async Task<Result<CreateDraftPolicyResponse>> CreateDraftPolicyAsync(
        CreateDraftPolicyRequest request,
        CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.Policy.ClientId, ct);
        if (client == null)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Client not found");

        var building = await _buildings.GetByIdAsync(request.Policy.BuildingId, ct);
        if (building == null || building.OwnerClientId != client.Id)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Building not found");

        var broker = await _brokers.GetByIdAsync(request.Policy.BrokerId, ct);
        if (broker == null)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Broker not found");

        var currency = await _currencies.GetByCodeAsync(request.Policy.CurrencyCode, ct);
        if (currency == null || !currency.IsActive)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Currency not found or inactive");

        var city = await _cities.GetByIdAsync(building.CityId, ct);
        var county = await _counties.GetByIdAsync(city!.CountyId, ct);
        var country = await _countries.GetByIdAsync(county!.CountryId, ct);

        var basePremium = request.Policy.BasePremium.MapToDomain();
        var tenure = request.Policy.Tenure.MapToDomain();

        var premiumRules = await _premiumRules.GetActiveAsync(ct);

        var draftContext = new PolicyDraftContext
        (
            broker.Id,
            broker.CommissionPercentage,
            country!.Id,
            county.Id,
            city.Id,
            building.BuildingType,
            building.ZoneRiskCategories,
            basePremium,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var finalPremium = _premiumCalculatorService.CalculateFinalPremium(draftContext, premiumRules);

        var draft = Policy.CreateDraft(
            client.Id,
            building.Id,
            broker.Id,
            tenure,
            basePremium,
            currency.Code,
            finalPremium,
            DateOnly.FromDateTime(DateTime.Now)
            );

        _policies.Add(draft, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.Conflict, "Duplicate policy number");
        }

        var response = new CreateDraftPolicyResponse(PolicyListItemDto.From(draft));
        return Result<CreateDraftPolicyResponse>.Ok(response);
    }

    public async Task<Result<ActivatePolicyResponse>> ActivatePolicyAsync(
        Guid policyNumber,
        CancellationToken ct = default)
    {
        var policy = await _policies.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<ActivatePolicyResponse>.Fail(ErrorType.NotFound, "Policy not found");

        policy.Activate();

        await _policies.UpdateAsync(policy, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new ActivatePolicyResponse(PolicyListItemDto.From(policy));
        return Result<ActivatePolicyResponse>.Ok(response);
    }

    public async Task<Result<CancelPolicyResponse>> CancelPolicyAsync(
        Guid policyNumber,
        CancelPolicyRequest request,
        CancellationToken ct = default)
    {
        var policy = await _policies.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<CancelPolicyResponse>.Fail(ErrorType.NotFound, "Policy not found");

        policy.Cancel(request.Reason, request.CancellationEffectiveDate);

        await _policies.UpdateAsync(policy, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new CancelPolicyResponse(PolicyListItemDto.From(policy));
        return Result<CancelPolicyResponse>.Ok(response);
    }

    public async Task<Result<GetPolicyDetailsResponse>> GetPolicyDetailsAsync(
        Guid requestPolicyNumber,
        CancellationToken ct = default)
    {
        var policy = await _policies.GetByIdAsync(requestPolicyNumber, ct);
        if (policy == null)
            return Result<GetPolicyDetailsResponse>.Fail(ErrorType.NotFound, "Policy not found");

        var client = await _clients.GetByIdAsync(policy.ClientId, ct);
        var building = await _buildings.GetByIdAsync(policy.BuildingId, ct);
        var broker = await _brokers.GetByIdAsync(policy.BrokerId, ct);

        var response = new GetPolicyDetailsResponse(PolicyDetailedDto.From(
            policy,
            ClientListItemDto.From(client!),
            BuildingListItemDto.From(building!),
            BrokerListItemDto.From(broker!)));

        return Result<GetPolicyDetailsResponse>.Ok(response);
    }

    public async Task<Result<ListPoliciesResponse>> ListPoliciesAsync(
        ListPoliciesRequest request,
        CancellationToken ct = default)
    {
        var searchCriteria = new PolicySearchCriteria(
            request.ClientId,
            request.BrokerId,
            request.Status,
            request.StartDate,
            request.EndDate);

        var searchResult = await _policies.SearchAsync(
            searchCriteria,
            request.Page,
            ct);

        var response = new ListPoliciesResponse(searchResult.Select(PolicyListItemDto.From).ToArray());
        return Result<ListPoliciesResponse>.Ok(response);
    }
}