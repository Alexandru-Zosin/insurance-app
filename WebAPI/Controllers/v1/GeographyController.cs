using Application.Services.Geography.DTOs;
using Application.Services.Geography;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[Route("api/brokers")]
public sealed class GeographyController(IGeographyService _geographyService) : BaseApiController
{
    [HttpGet("countries")]
    public async Task<ActionResult<GetCountriesResponse>> GetCountries(
        CancellationToken ct)
    {
        var result = await _geographyService.GetCountriesAsync(
            new GetCountriesRequest(), ct);

        return FromResult(result);
    }

    [HttpGet("countries/{countryId:int}/counties")]
    public async Task<ActionResult<GetCountiesByCountryResponse>> GetCountiesByCountryId(
        int countryId,
        CancellationToken ct)
    {
        var result = await _geographyService.GetCountiesByCountryAsync(
            new GetCountiesByCountryRequest(countryId), ct);

        return FromResult(result);
    }

    [HttpGet("counties/{countyId:int}/cities")]
    public async Task<ActionResult<GetCitiesByCountyResponse>> GetCitiesByCountyId(
        int countyId,
        CancellationToken ct)
    {
        var result = await _geographyService.GetCitiesByCountyAsync(
            new GetCitiesByCountyRequest(countyId), ct);

        return FromResult(result);
    }
}
