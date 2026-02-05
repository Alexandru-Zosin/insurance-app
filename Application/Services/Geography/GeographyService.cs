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
        GetCitiesByCountyRequest request,
        CancellationToken ct = default)
    {
        var cities = await _cities.GetByCountyIdAsync(
            request.CountyId,
            ct);

        return Result<GetCitiesByCountyResponse>.Ok(
           new GetCitiesByCountyResponse(
               cities.Select(CityListItemDto.From).ToList()));
    }

    public async Task<Result<GetCountiesByCountryResponse>> GetCountiesByCountryAsync(
        GetCountiesByCountryRequest request,
        CancellationToken ct = default)
    {
        var country = await _countries.GetByIdAsync(
            request.CountryId,
            ct);

        if (country == null)
        {
            return Result<GetCountiesByCountryResponse>.Fail(
                ErrorType.NotFound,
                "Country not found");
        }

        var counties = await _counties.GetByCountryIdAsync(
            request.CountryId,
            ct);

        return Result<GetCountiesByCountryResponse>.Ok(
            new GetCountiesByCountryResponse(
                counties.Select(CountyListItemDto.From).ToList()));
    }

    public async Task<Result<GetCountriesResponse>> GetCountriesAsync(
       GetCountriesRequest _,
       CancellationToken ct = default)
    {
        var countries = await _countries.GetAllAsync(ct);

        return Result<GetCountriesResponse>.Ok(
           new GetCountriesResponse(
               countries.Select(CountryListItemDto.From).ToList()));
    }

}
