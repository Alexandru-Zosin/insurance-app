using Application.Services.Geography.DTOs;
using Application.Services.Geography;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class GeographyController(IGeographyService GeographyService) : ApiController
{
    [HttpGet("countries")]
    public async Task<ActionResult<GetCountriesResponse>> GetCountriesAsync(CancellationToken ct = default)
    {
        var result = await GeographyService.GetCountriesAsync(ct);

        return FromResult(result);
    }

    [HttpGet("countries/{countryId:int}/counties")]
    public async Task<ActionResult<GetCountiesByCountryResponse>> GetCountiesByCountryAsync(
        [FromRoute] int countryId,
        CancellationToken ct = default)
    {
        var result = await GeographyService.GetCountiesByCountryAsync(countryId, ct);

        return FromResult(result);
    }

    [HttpGet("counties/{countyId:int}/cities")]
    public async Task<ActionResult<GetCitiesByCountyResponse>> GetCitiesByCountyAsync(
        [FromRoute] int countyId,
        CancellationToken ct = default)
    {
        var result = await GeographyService.GetCitiesByCountyAsync(countyId, ct);

        return FromResult(result);
    }
}