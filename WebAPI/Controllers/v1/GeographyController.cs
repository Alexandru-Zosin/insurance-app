using Application.UseCases.Geography;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[Route("api/brokers")]
public sealed class GeographyController : BaseApiController
{
    private readonly GetCountriesService _countries;
    private readonly GetCountiesByCountryService _counties;
    private readonly GetCitiesByCountyService _cities;

    public GeographyController(
        GetCountriesService countries,
        GetCountiesByCountryService counties,
        GetCitiesByCountyService cities)
    {
        _countries = countries;
        _counties = counties;
        _cities = cities;
    }

    [HttpGet("countries")]
    public async Task<ActionResult<GetCountriesService.Response>> GetCountries(
        CancellationToken ct)
    {
        var result = await _countries.HandleAsync(
            new GetCountriesService.Request(), ct);

        return FromResult(result);
    }

    [HttpGet("countries/{countryId:int}/counties")]
    public async Task<ActionResult<GetCountiesByCountryService.Response>> GetCounties(
        int countryId,
        CancellationToken ct)
    {
        var result = await _counties.HandleAsync(
            new GetCountiesByCountryService.Request(countryId), ct);

        return FromResult(result);
    }

    [HttpGet("counties/{countyId:int}/cities")]
    public async Task<ActionResult<GetCitiesByCountyService.Response>> GetCities(
        int countyId,
        CancellationToken ct)
    {
        var result = await _cities.HandleAsync(
            new GetCitiesByCountyService.Request(countyId), ct);

        return FromResult(result);
    }
}
