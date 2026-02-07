using Application.Services.Metadata.DTOs;
using Application.Services.Risks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/risk-factors")]
public sealed class RiskConfigurationsController(IRiskConfigurationService RiskService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListRiskConfigurationsResponse>> List(CancellationToken ct = default)
    {
        var result = await RiskService.ListAsync(ct);
        return FromResult(result);
    }

    [HttpPost("country")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateCountry(
        [FromBody] CreateCountryRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await RiskService.CreateCountryAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("county")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateCounty(
        [FromBody] CreateCountyRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await RiskService.CreateCountyAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("city")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateCity(
        [FromBody] CreateCityRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await RiskService.CreateCityAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("building-type")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateBuildingType(
        [FromBody] CreateBuildingTypeRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await RiskService.CreateBuildingTypeAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("zone-category")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateZoneCategory(
        [FromBody] CreateZoneCategoryRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await RiskService.CreateZoneCategoryAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPut("{riskConfigurationId:guid}")]
    public async Task<ActionResult<UpdateRiskConfigurationResponse>> UpdateCore(
        [FromRoute] Guid riskConfigurationId,
        [FromBody] UpdateRiskConfigurationRequest request,
        CancellationToken ct = default)
    {
        var result = await RiskService.UpdateCoreAsync(riskConfigurationId, request, ct);
        return FromResult(result);
    }
}
