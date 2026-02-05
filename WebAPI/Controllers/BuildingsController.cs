using Application.Services.Buildings;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Application.Services.Buildings.DTOs;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class BuildingsController(IBuildingService BuildingService) : ApiController
{
    [HttpGet("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<GetBuildingsForClientResponse>> GetForClient(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await BuildingService.GetBuildingsForClientAsync(
            new GetBuildingsForClientRequest(clientId), ct);

        return FromResult(result);
    }

    [HttpGet("buildings/{buildingId:guid}")]
    public async Task<ActionResult<GetBuildingDetailsResponse>> Get(
        Guid buildingId,
        CancellationToken ct)
    {
        var result = await BuildingService.GetBuildingDetailsAsync(
            new GetBuildingDetailsRequest(buildingId), ct);

        return FromResult(result);
    }

    [HttpPost("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<RegisterBuildingResponse>> Register(
        [FromBody] RegisterBuildingRequest request,
        CancellationToken ct)
    {
        var result = await BuildingService.RegisterBuildingAsync(request, ct);

        return FromCreated(
            result,
            $"/api/brokers/buildings/{result.Value!.BuildingId}");
    }

    [HttpPut("buildings/{buildingId:guid}")]
    public async Task<ActionResult<UpdateBuildingResponse>> Update(
        [FromBody] UpdateBuildingRequest request,
        CancellationToken ct)
    {
        var result = await BuildingService.UpdateBuildingAsync(
            request, ct);

        return FromResult(result);
    }
}