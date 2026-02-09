using Application.Services.Metadata.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/fees")]
public sealed class FeeConfigurationsController(IFeeConfigurationService FeeConfigService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListFeeConfigurationsResponse>> ListFeeConfigurationsAsync(CancellationToken ct = default)
    {
        var result = await FeeConfigService.ListFeeConfigurationsAsync(ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateFeeConfigurationResponse>> CreateFeeConfigurationAsync(
        [FromBody] CreateFeeConfigurationRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.CreateFeeConfigurationAsync(request, ct);
        
        return FromCreated(result, $"/api/admin/fees/{result.Value!.FeeConfig.Id}");
    }

    [HttpPut("{feeConfigId:guid}")]
    public async Task<ActionResult<UpdateFeeConfigurationResponse>> UpdateFeeConfigurationAsync(
        [FromRoute] Guid feeConfigId,
        [FromBody] UpdateFeeConfigurationRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.UpdateFeeConfigurationAsync(feeConfigId, request, ct);

        return FromResult(result);
    }

    [HttpPost("{feeConfigId:guid}/activate")]
    public async Task<ActionResult<SetFeeConfigStatusResponse>> ActivateFeeConfigurationAsync(
        [FromRoute] Guid feeConfigId,
        CancellationToken ct)
    {
        var result = await FeeConfigService.SetFeeConfigurationStatusAsync(feeConfigId, true, ct);

        return FromResult(result);
    }

    [HttpPost("{feeConfigId:guid}/deactivate")]
    public async Task<ActionResult<SetFeeConfigStatusResponse>> DeactivateFeeConfigurationAsync(
        [FromRoute] Guid feeConfigId,
        CancellationToken ct)
    {
        var result = await FeeConfigService.SetFeeConfigurationStatusAsync(feeConfigId, false, ct);

        return FromResult(result);
    }
}
