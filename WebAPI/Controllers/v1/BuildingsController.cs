using Application.Services.Buildings;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Application.Services.Buildings.DTOs;

[Route("api/brokers")]
public sealed class BuildingsController(IBuildingService _buildingService) : BaseApiController
{
    [HttpGet("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<GetBuildingsForClientResponse>> GetForClient(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await _buildingService.GetBuildingsForClientAsync(
            new GetBuildingsForClientRequest(clientId), ct);

        return FromResult(result);
    }

    [HttpGet("buildings/{buildingId:guid}")]
    public async Task<ActionResult<GetBuildingDetailsResponse>> Get(
        Guid buildingId,
        CancellationToken ct)
    {
        var result = await _buildingService.GetBuildingDetailsAsync(
            new GetBuildingDetailsRequest(buildingId), ct);

        return FromResult(result);
    }

    [HttpPost("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<RegisterBuildingResponse>> Register(
        Guid clientId,
        [FromBody] RegisterBuildingRequest request,
        CancellationToken ct)
    {
        var result = await _buildingService.RegisterBuildingAsync(
            request with { ClientId = clientId }, ct);

        return FromCreated(
            result,
            $"/api/brokers/buildings/{result.Value!.BuildingId}");
    }

    [HttpPut("buildings/{buildingId:guid}")]
    public async Task<ActionResult<UpdateBuildingResponse>> Update(
        Guid buildingId,
        [FromBody] UpdateBuildingRequest request,
        CancellationToken ct)
    {
        var result = await _buildingService.UpdateBuildingAsync(
            request with { BuildingId = buildingId }, ct);

        return FromResult(result);
    }
}
