using Application.Common;
using Application.Repositories;
using Application.Services.Policies.DTOs;

namespace Application.Services.Policies;

public sealed class PolicyDraftPrerequisitesLoader(
    IClientRepository clientRepository,
    IBuildingRepository buildingRepository,
    IBrokerRepository brokerRepository,
    ICurrencyRepository currencyRepository,
    ICityRepository cityRepository,
    ICountyRepository countyRepository,
    ICountryRepository countryRepository) : IPolicyDraftPrerequisitesLoader
{
    public async Task<Result<PolicyDraftPrerequisites>> LoadAsync(
        CreateDraftPolicyRequest request,
        CancellationToken ct = default)
    {
        var draftClient = await clientRepository.GetByIdAsync(request.Policy.ClientId, ct);
        if (draftClient is null)
            return Result<PolicyDraftPrerequisites>.Fail(ErrorType.NotFound, "Client not found");

        var draftBuilding = await buildingRepository.GetByIdAsync(request.Policy.BuildingId, ct);
        if (draftBuilding is null || draftBuilding.OwnerClientId != draftClient.Id)
            return Result<PolicyDraftPrerequisites>.Fail(ErrorType.NotFound, "Building not found");

        var draftBroker = await brokerRepository.GetByIdAsync(request.Policy.BrokerId, ct);
        if (draftBroker is null)
            return Result<PolicyDraftPrerequisites>.Fail(ErrorType.NotFound, "Broker not found");

        var draftCurrency = await currencyRepository.GetByCodeAsync(request.Policy.CurrencyCode, ct);
        if (draftCurrency is null || !draftCurrency.IsActive)
            return Result<PolicyDraftPrerequisites>.Fail(ErrorType.NotFound, "Currency not found or inactive");

        var draftCity = await cityRepository.GetByIdAsync(draftBuilding.CityId, ct);
        var draftCounty = await countyRepository.GetByIdAsync(draftCity!.CountyId, ct);
        var draftCountry = await countryRepository.GetByIdAsync(draftCounty!.CountryId, ct);

        return Result<PolicyDraftPrerequisites>.Ok(
            new PolicyDraftPrerequisites(
                draftClient,
                draftBuilding,
                draftBroker,
                draftCurrency,
                draftCountry!.Id,
                draftCounty.Id,
                draftCity.Id));
    }
}
