using Application.Services.Metadata.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/fees")]
public sealed class FeeConfigurationsController(IFeeConfigurationService FeeConfigService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListFeeConfigurationsResponse>> List(
        CancellationToken ct = default)
    {
        var result = await FeeConfigService.ListFeeConfigsAsync(
            new ListFeeConfigurationsRequest(), ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateFeeConfigurationResponse>> Create(
        [FromBody] CreateFeeConfigurationRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.CreateFeeConfigAsync(request, ct);
        
        return FromCreated(
            result,
            $"/api/admin/fees/{result.Value!.FeeConfig.Id}");
    }

    [HttpPut("{feeConfigId:guid}")]
    public async Task<ActionResult<UpdateFeeConfigurationResponse>> Update(
        [FromRoute] Guid feeConfigId,
        [FromBody] UpdateFeeConfigurationRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.UpdateFeeConfigAsync(feeConfigId, request, ct);

        return FromResult(result);
    }

    [HttpPut("{feeConfigId:guid}/status")]
    public async Task<ActionResult<SetFeeConfigStatusResponse>> SetStatus(
        [FromRoute] Guid feeConfigId,
        [FromBody] SetFeeConfigStatusRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.SetFeeConfigStatusAsync(feeConfigId, request, ct);

        return FromResult(result);
    }
}
