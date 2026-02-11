using Application.Services.Metadata.DTOs;
using Application.Services.Risks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/risk-factors")]
public sealed class RiskConfigurationsController(IRiskConfigurationService riskConfigurationService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListRiskConfigurationsResponse>> ListRiskConfigurationsAsync(CancellationToken ct = default)
    {
        var result = await riskConfigurationService.ListRiskConfigurationsAsync(ct);
        return FromResult(result);
    }

    [HttpPost("country")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateCountryRiskConfigurationAsync(
        [FromBody] CreateCountryRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await riskConfigurationService.CreateCountryRiskConfigurationAsync(request, ct);

        return FromCreated(result, $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("county")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateCountyRiskConfigurationAsync(
        [FromBody] CreateCountyRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await riskConfigurationService.CreateCountyRiskConfigurationAsync(request, ct);

        return FromCreated(result, $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("city")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateCityRiskConfigurationAsync(
        [FromBody] CreateCityRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await riskConfigurationService.CreateCityRiskConfigurationAsync(request, ct);

        return FromCreated(result, $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("building-type")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateBuildingTypeRiskConfigurationAsync(
        [FromBody] CreateBuildingTypeRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await riskConfigurationService.CreateBuildingTypeRiskConfigurationAsync(request, ct);

        return FromCreated(result, $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPost("zone-category")]
    public async Task<ActionResult<CreateRiskConfigurationResponse>> CreateZoneCategoryRiskConfigurationAsync(
        [FromBody] CreateZoneCategoryRiskRequest request,
        CancellationToken ct = default)
    {
        var result = await riskConfigurationService.CreateZoneCategoryRiskConfigurationAsync(request, ct);

        return FromCreated(result, $"/api/admin/risk-factors/{result.Value!.RiskConfiguration.Id}");
    }

    [HttpPut("{riskConfigurationId:guid}")]
    public async Task<ActionResult<UpdateRiskConfigurationResponse>> UpdateRiskConfigurationCoreAsync(
        [FromRoute] Guid riskConfigurationId,
        [FromBody] UpdateRiskConfigurationRequest request,
        CancellationToken ct = default)
    {
        var result = await riskConfigurationService.UpdateRiskConfigurationAsync(riskConfigurationId, request, ct);
        return FromResult(result);
    }
}
