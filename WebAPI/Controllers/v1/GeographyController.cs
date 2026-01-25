using Application.Services.Geography.DTO;
using Application.UseCases.Geography;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[Route("api/brokers")]
public sealed class GeographyController : BaseApiController
{
    private readonly GetCountries _countries;
    private readonly GetCountiesByCountryService _counties;
    private readonly GetCitiesByCountyService _cities;

    public GeographyController(
        GetCountries countries,
        GetCountiesByCountryService counties,
        GetCitiesByCountyService cities)
    {
        _countries = countries;
        _counties = counties;
        _cities = cities;
    }

    [HttpGet("countries")]
    public async Task<ActionResult<GetCountriesResponse>> GetCountries(
        CancellationToken ct)
    {
        var result = await _countries.HandleAsync(
            new GetCountriesRequest(), ct);

        return FromResult(result);
    }

    [HttpGet("countries/{countryId:int}/counties")]
    public async Task<ActionResult<GetCountiesByCountryResponse>> GetCounties(
        int countryId,
        CancellationToken ct)
    {
        var result = await _counties.HandleAsync(
            new GetCountiesByCountryRequest(countryId), ct);

        return FromResult(result);
    }

    [HttpGet("counties/{countyId:int}/cities")]
    public async Task<ActionResult<GetCitiesByCountyResponse>> GetCities(
        int countyId,
        CancellationToken ct)
    {
        var result = await _cities.HandleAsync(
            new GetCitiesByCountyRequest(countyId), ct);

        return FromResult(result);
    }
}
