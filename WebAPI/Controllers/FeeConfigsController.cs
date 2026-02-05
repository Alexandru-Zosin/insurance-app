using Application.Common;
using Application.Services.Metadata.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/fees")]
public sealed class FeeConfigsController(IFeeConfigService FeeConfigService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListFeeConfigsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var pagedRequest = new PageRequest(page, pageSize);
        var result = await FeeConfigService.ListFeeConfigsAsync(
            new ListFeeConfigsRequest(pagedRequest), ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateFeeConfigResponse>> Create(
        [FromBody] CreateFeeConfigRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.CreateFeeConfigAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/fees/{result.Value!.FeeConfig.Id}");
    }

    [HttpPut("{feeConfigId:guid}")]
    public async Task<ActionResult<UpdateFeeConfigResponse>> Update(
        [FromBody] UpdateFeeConfigRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.UpdateFeeConfigAsync(request, ct);

        return FromResult(result);
    }

    [HttpPut("{feeConfigId:guid}/status")]
    public async Task<ActionResult<SetFeeConfigStatusResponse>> SetStatus(
        [FromBody] SetFeeConfigStatusRequest request,
        CancellationToken ct)
    {
        var result = await FeeConfigService.SetFeeConfigStatusAsync(request, ct);

        return FromResult(result);
    }
}
