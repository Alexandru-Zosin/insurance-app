using Application.Common;
using Application.Repositories;
using Application.Services.Policies.DTOs;
using Application.Services.Shared.DTOs.BrokerDTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Configurations;
using Domain.Policies;
using Domain.Services;
using Domain.Shared;

namespace Application.Services.Policies;

public sealed class PolicyService(
    IPolicyRepository _policyRepository,
    IClientRepository _clientRepository,
    IBuildingRepository _buildingRepository,
    IBrokerRepository _brokerRepository,
    ICurrencyRepository _currencyRepository,
    ICityRepository _cityRepository,
    ICountyRepository _countyRepository,
    ICountryRepository _countryRepository,
    IFeeConfigurationRepository _feeConfigurationRepository,
    IRiskConfigurationRepository _riskConfigurationRepository,
    IPremiumCalculatorService _premiumCalculatorService,
    IUnitOfWork _uow) : IPolicyService
{
    public async Task<Result<CreateDraftPolicyResponse>> CreateDraftPolicyAsync(
        CreateDraftPolicyRequest request,
        CancellationToken ct = default)
    {
        var draftClient = await _clientRepository.GetByIdAsync(request.Policy.ClientId, ct);
        if (draftClient == null)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Client not found");

        var draftBuilding = await _buildingRepository.GetByIdAsync(request.Policy.BuildingId, ct);
        if (draftBuilding == null || draftBuilding.OwnerClientId != draftClient.Id)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Building not found");

        var draftBroker = await _brokerRepository.GetByIdAsync(request.Policy.BrokerId, ct);
        if (draftBroker == null)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Broker not found");

        var draftCurrency = await _currencyRepository.GetByCodeAsync(request.Policy.CurrencyCode, ct);
        if (draftCurrency == null || !draftCurrency.IsActive)
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.NotFound, "Currency not found or inactive");

        var draftCity = await _cityRepository.GetByIdAsync(draftBuilding.CityId, ct);
        var draftCounty = await _countyRepository.GetByIdAsync(draftCity!.CountyId, ct);
        var draftCountry = await _countryRepository.GetByIdAsync(draftCounty!.CountryId, ct);

        var draftBasePremium = request.Policy.BasePremium.MapToDomain();
        var draftTenure = request.Policy.Tenure.MapToDomain();

        var draftContext = new PolicyDraftContext
        (
            draftBroker.Id,
            draftBroker.CommissionPercentage,
            draftCountry!.Id,
            draftCounty.Id,
            draftCity.Id,
            draftBuilding.BuildingType,
            draftBuilding.ZoneRiskCategories,
            draftBasePremium,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );
        var draftFinalPremium = await CalculateDraftFinalPremiumAsync(draftContext, ct);

        var newDraft = Policy.CreateDraft(
            draftClient.Id,
            draftBuilding.Id,
            draftBroker.Id,
            draftTenure,
            draftBasePremium,
            draftCurrency.Code,
            draftFinalPremium,
            DateOnly.FromDateTime(DateTime.Now)
            );

        _policyRepository.Add(newDraft, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<CreateDraftPolicyResponse>.Fail(ErrorType.Conflict, "Duplicate policy number");
        }

        var response = new CreateDraftPolicyResponse(PolicyListItemDto.From(newDraft));
        return Result<CreateDraftPolicyResponse>.Ok(response);
    }

    private async Task<Money> CalculateDraftFinalPremiumAsync(
        PolicyDraftContext draftContext,
        CancellationToken ct = default)
    {
        if (draftContext is null) throw new ArgumentNullException(nameof(draftContext));

        var feeConfigurations = await _feeConfigurationRepository.ListAsync(ct);
        var riskConfigurations = await _riskConfigurationRepository.ListAsync(ct);

        var activePremiumRules = feeConfigurations
            .Where(fee => fee.IsActive)
            .Cast<IPremiumRule>()
            .Concat(riskConfigurations.Where(risk => risk.Core.IsActive)
                                      .Cast<IPremiumRule>())
            .ToArray();

        return _premiumCalculatorService.CalculateFinalPremium(draftContext, activePremiumRules);
    }

    public async Task<Result<ActivatePolicyResponse>> ActivatePolicyAsync(
        Guid policyNumber,
        CancellationToken ct = default)
    {
        var policy = await _policyRepository.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<ActivatePolicyResponse>.Fail(ErrorType.NotFound, "Policy not found");

        policy.Activate();

        await _policyRepository.UpdateAsync(policy, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new ActivatePolicyResponse(PolicyListItemDto.From(policy));
        return Result<ActivatePolicyResponse>.Ok(response);
    }

    public async Task<Result<CancelPolicyResponse>> CancelPolicyAsync(
        Guid policyNumber,
        CancelPolicyRequest request,
        CancellationToken ct = default)
    {
        var policy = await _policyRepository.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<CancelPolicyResponse>.Fail(ErrorType.NotFound, "Policy not found");

        policy.Cancel(request.Reason, request.CancellationEffectiveDate);

        await _policyRepository.UpdateAsync(policy, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new CancelPolicyResponse(PolicyListItemDto.From(policy));
        return Result<CancelPolicyResponse>.Ok(response);
    }

    public async Task<Result<GetPolicyDetailsResponse>> GetPolicyDetailsAsync(
        Guid policyNumber,
        CancellationToken ct = default)
    {
        var policy = await _policyRepository.GetByIdAsync(policyNumber, ct);
        if (policy == null)
            return Result<GetPolicyDetailsResponse>.Fail(ErrorType.NotFound, "Policy not found");

        var policyClient = await _clientRepository.GetByIdAsync(policy.ClientId, ct);
        var policyBuilding = await _buildingRepository.GetByIdAsync(policy.BuildingId, ct);
        var policyBroker = await _brokerRepository.GetByIdAsync(policy.BrokerId, ct);

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
            request.Status,
            request.StartDate,
            request.EndDate);

        var matchedPolicies = await _policyRepository.SearchAsync(
            criteria,
            request.Page,
            ct);

        var response = new ListPoliciesResponse(matchedPolicies.Select(PolicyListItemDto.From).ToArray());
        return Result<ListPoliciesResponse>.Ok(response);
    }
}