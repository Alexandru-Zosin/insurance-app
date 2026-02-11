using Application.Services.Metadata.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/fees")]
public sealed class FeeConfigurationsController(IFeeConfigurationService feeConfigurationService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListFeeConfigurationsResponse>> ListFeeConfigurationsAsync(CancellationToken ct = default)
    {
        var result = await feeConfigurationService.ListFeeConfigurationsAsync(ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateFeeConfigurationResponse>> CreateFeeConfigurationAsync(
        [FromBody] CreateFeeConfigurationRequest request,
        CancellationToken ct)
    {
        var result = await feeConfigurationService.CreateFeeConfigurationAsync(request, ct);
        
        return FromCreated(result, $"/api/admin/fees/{result.Value!.FeeConfig.Id}");
    }

    [HttpPut("{feeConfigId:guid}")]
    public async Task<ActionResult<UpdateFeeConfigurationResponse>> UpdateFeeConfigurationAsync(
        [FromRoute] Guid feeConfigId,
        [FromBody] UpdateFeeConfigurationRequest request,
        CancellationToken ct)
    {
        var result = await feeConfigurationService.UpdateFeeConfigurationAsync(feeConfigId, request, ct);

        return FromResult(result);
    }

    [HttpPost("{feeConfigId:guid}/activate")]
    public async Task<ActionResult<SetFeeConfigStatusResponse>> ActivateFeeConfigurationAsync(
        [FromRoute] Guid feeConfigId,
        CancellationToken ct)
    {
        var result = await feeConfigurationService.SetFeeConfigurationStatusAsync(feeConfigId, true, ct);

        return FromResult(result);
    }

    [HttpPost("{feeConfigId:guid}/deactivate")]
    public async Task<ActionResult<SetFeeConfigStatusResponse>> DeactivateFeeConfigurationAsync(
        [FromRoute] Guid feeConfigId,
        CancellationToken ct)
    {
        var result = await feeConfigurationService.SetFeeConfigurationStatusAsync(feeConfigId, false, ct);

        return FromResult(result);
    }
}
