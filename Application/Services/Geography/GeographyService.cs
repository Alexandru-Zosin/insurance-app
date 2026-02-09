using Application.Repositories;
using Application.Services.Geography.DTOs;
using Application.Common;
using Application.Services.Shared.DTOs.GeographyDTOs;

namespace Application.Services.Geography;

public sealed class GeographyService(
    ICityRepository _cities,
    ICountyRepository _counties,
    ICountryRepository _countries
    ) : IGeographyService
{
    public async Task<Result<GetCitiesByCountyResponse>> GetCitiesByCountyAsync(
        int requestCountyId,
        CancellationToken ct = default)
    {
        var cities = await _cities.GetByCountyIdAsync(requestCountyId, ct);

        var response = new GetCitiesByCountyResponse(cities.Select(CityListItemDto.From).ToList());
        return Result<GetCitiesByCountyResponse>.Ok(response);
    }

    public async Task<Result<GetCountiesByCountryResponse>> GetCountiesByCountryAsync(
        int requestCountryId,
        CancellationToken ct = default)
    {
        var country = await _countries.GetByIdAsync(requestCountryId, ct);

        if (country == null)
        {
            return Result<GetCountiesByCountryResponse>.Fail(
                ErrorType.NotFound,
                "Country not found");
        }

        var counties = await _counties.GetByCountryIdAsync(requestCountryId, ct);

        var response = new GetCountiesByCountryResponse(counties.Select(CountyListItemDto.From).ToList());
        return Result<GetCountiesByCountryResponse>.Ok(response);
    }

    public async Task<Result<GetCountriesResponse>> GetCountriesAsync(CancellationToken ct = default)
    {
        var countries = await _countries.GetAllAsync(ct);

        var response = new GetCountriesResponse(countries.Select(CountryListItemDto.From).ToList());
        return Result<GetCountriesResponse>.Ok(response);
    }

}
