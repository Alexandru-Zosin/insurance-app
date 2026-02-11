using Application.Services.Reports;
using Application.Services.Reports.DTOs;
using Domain.Buildings;
using Domain.Policies;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/reports")]
public class AdminReportsController(IReportService reportService) : ApiController
{
    [HttpGet("policies-by-country")]
    public async Task<ActionResult<GetReportsResponse>> GetReportsByCountryAsync(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] PolicyStatus? status,
        [FromQuery] string? currencyCode,
        [FromQuery] BuildingType? buildingType,
        CancellationToken ct = default
        )
    {
        var result = await reportService.GetReportsByCountryAsync(
            new GetReportsCriteria(from, to, status, currencyCode, buildingType), ct);

        return FromResult(result);

    }

    [HttpGet("policies-by-county")]
    public async Task<ActionResult<GetReportsResponse>> GetReportsByCountyAsync(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] PolicyStatus? status,
        [FromQuery] string? currencyCode,
        [FromQuery] BuildingType? buildingType,
        CancellationToken ct = default
        )
    {
        var result = await reportService.GetReportsByCountyAsync(
            new GetReportsCriteria(from, to, status, currencyCode, buildingType), ct);

        return FromResult(result);
    }

    [HttpGet("policies-by-city")]
    public async Task<ActionResult<GetReportsResponse>> GetReportsByCityAsync(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] PolicyStatus? status,
        [FromQuery] string? currencyCode,
        [FromQuery] BuildingType? buildingType,
        CancellationToken ct = default
        )
    {
        var result = await reportService.GetReportsByCityAsync(
            new GetReportsCriteria(from, to, status, currencyCode, buildingType), ct);

        return FromResult(result);
    }

    [HttpGet("policies-by-broker")]
    public async Task<ActionResult<GetReportsResponse>> GetReportsByBrokerAsync(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] PolicyStatus? status,
        [FromQuery] string? currencyCode,
        [FromQuery] BuildingType? buildingType,
        CancellationToken ct = default
        )
    {
        var result = await reportService.GetReportsByBrokerAsync(
            new GetReportsCriteria(from, to, status, currencyCode, buildingType), ct);

        return FromResult(result);
    }
}
