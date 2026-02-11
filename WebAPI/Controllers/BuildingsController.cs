using Application.Services.Buildings;
using Microsoft.AspNetCore.Mvc;
using Application.Services.Buildings.DTOs;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class BuildingsController(IBuildingService buildingService) : ApiController
{
    [HttpGet("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(
        [FromRoute] Guid clientId,
        CancellationToken ct)
    {
        var result = await buildingService.GetBuildingsForClientAsync(clientId, ct);

        return FromResult(result);
    }

    [HttpGet("buildings/{buildingId:guid}")]
    public async Task<ActionResult<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(
        [FromRoute] Guid buildingId,
        CancellationToken ct)
    {
        var result = await buildingService.GetBuildingDetailsAsync(buildingId, ct);

        return FromResult(result);
    }

    [HttpPost("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<RegisterBuildingResponse>> RegisterBuildingAsync(
        [FromBody] RegisterBuildingRequest request,
        CancellationToken ct)
    {
        var result = await buildingService.RegisterBuildingAsync(request, ct);

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
        var result = await buildingService.UpdateBuildingAsync(buildingId, request, ct);

        return FromResult(result);
    }
}