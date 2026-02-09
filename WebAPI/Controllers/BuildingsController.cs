using Application.Services.Buildings;
using Microsoft.AspNetCore.Mvc;
using Application.Services.Buildings.DTOs;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class BuildingsController(IBuildingService BuildingService) : ApiController
{
    [HttpGet("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(
        [FromRoute] Guid clientId,
        CancellationToken ct)
    {
        var result = await BuildingService.GetBuildingsForClientAsync(clientId, ct);

        return FromResult(result);
    }

    [HttpGet("buildings/{buildingId:guid}")]
    public async Task<ActionResult<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(
        [FromRoute] Guid buildingId,
        CancellationToken ct)
    {
        var result = await BuildingService.GetBuildingDetailsAsync(buildingId, ct);

        return FromResult(result);
    }

    [HttpPost("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<RegisterBuildingResponse>> RegisterBuildingAsync(
        [FromBody] RegisterBuildingRequest request,
        CancellationToken ct)
    {
        var result = await BuildingService.RegisterBuildingAsync(request, ct);

        return FromCreated(
            result,
            $"/api/brokers/buildings/{result.Value!.BuildingId}");
    }

    [HttpPut("buildings/{buildingId:guid}")]
    public async Task<ActionResult<UpdateBuildingResponse>> UpdateBuildingAsync(
        [FromRoute] Guid buildingId,
        [FromBody] UpdateBuildingRequest request,
        CancellationToken ct)
    {
        var result = await BuildingService.UpdateBuildingAsync(buildingId, request, ct);

        return FromResult(result);
    }
}