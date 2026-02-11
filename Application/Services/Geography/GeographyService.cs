using Application.Repositories;
using Application.Services.Geography.DTOs;
using Application.Common;
using Application.Services.Shared.DTOs.GeographyDTOs;

namespace Application.Services.Geography;

public sealed class GeographyService(
    ICityRepository cityRepository,
    ICountyRepository countyRepository,
    ICountryRepository countryRepositories
    ) : IGeographyService
{
    public async Task<Result<GetCitiesByCountyResponse>> GetCitiesByCountyAsync(
        int countyId,
        CancellationToken ct = default)
    {
        var countyCities = await cityRepository.ListByCountyIdAsync(countyId, ct);

        var response = new GetCitiesByCountyResponse(countyCities.Select(CityListItemDto.From).ToList());
        return Result<GetCitiesByCountyResponse>.Ok(response);
    }

    public async Task<Result<GetCountiesByCountryResponse>> GetCountiesByCountryAsync(
        int countryId,
        CancellationToken ct = default)
    {
        var countryCounties = await countyRepository.ListByCountryIdAsync(countryId, ct);

        var response = new GetCountiesByCountryResponse(countryCounties
            .Select(CountyListItemDto.From).ToList());
        return Result<GetCountiesByCountryResponse>.Ok(response);
    }

    public async Task<Result<GetCountriesResponse>> GetCountriesAsync(CancellationToken ct = default)
    {
        var allCountries = await countryRepositories.ListAsync(ct);

        var response = new GetCountriesResponse(allCountries.Select(CountryListItemDto.From).ToList());
        return Result<GetCountriesResponse>.Ok(response);
    }

}
