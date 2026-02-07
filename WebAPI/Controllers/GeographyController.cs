using Application.Services.Geography.DTOs;
using Application.Services.Geography;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class GeographyController(IGeographyService GeographyService) : ApiController
{
    [HttpGet("countries")]
    public async Task<ActionResult<GetCountriesResponse>> GetCountries(
        CancellationToken ct)
    {
        var result = await GeographyService.GetCountriesAsync(
            new GetCountriesRequest(), ct);

        return FromResult(result);
    }

    [HttpGet("countries/{countryId:int}/counties")]
    public async Task<ActionResult<GetCountiesByCountryResponse>> GetCountiesByCountryId(
        int countryId,
        CancellationToken ct)
    {
        var result = await GeographyService.GetCountiesByCountryAsync(
            new GetCountiesByCountryRequest(countryId), ct);

        return FromResult(result);
    }

    [HttpGet("counties/{countyId:int}/cities")]
    public async Task<ActionResult<GetCitiesByCountyResponse>> GetCitiesByCountyId(
        int countyId,
        CancellationToken ct)
    {
        var result = await GeographyService.GetCitiesByCountyAsync(
            new GetCitiesByCountyRequest(countyId), ct);

        return FromResult(result);
    }
}