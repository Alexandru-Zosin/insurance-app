using Application.Services.Buildings.DTO;
using Application.Services.Buildings;
using Application.Services.Buildings.GetBuildingsForClient;
using Application.Services.Buildings.UpdateBuilding;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[Route("api/brokers")]
public sealed class BuildingsController : BaseApiController
{
    private readonly GetBuildingsForClientService _getForClient;
    private readonly GetBuildingDetailsService _getDetails;
    private readonly RegisterBuildingService _register;
    private readonly UpdateBuildingService _update;

    public BuildingsController(
        GetBuildingsForClientService getForClient,
        GetBuildingDetailsService getDetails,
        RegisterBuildingService register,
        UpdateBuildingService update)
    {
        _getForClient = getForClient;
        _getDetails = getDetails;
        _register = register;
        _update = update;
    }

    [HttpGet("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<GetBuildingsForClientResponse>> GetForClient(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await _getForClient.HandleAsync(
            new GetBuildingsForClientRequest(clientId), ct);

        return FromResult(result);
    }

    [HttpGet("buildings/{buildingId:guid}")]
    public async Task<ActionResult<GetBuildingDetailsResponse>> Get(
        Guid buildingId,
        CancellationToken ct)
    {
        var result = await _getDetails.HandleAsync(
            new GetBuildingDetailsRequest(buildingId), ct);

        return FromResult(result);
    }

    [HttpPost("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<RegisterBuildingResponse>> Register(
        Guid clientId,
        [FromBody] RegisterBuildingRequest request,
        CancellationToken ct)
    {
        var result = await _register.HandleAsync(
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
        var result = await _update.HandleAsync(
            request with { BuildingId = buildingId }, ct);

        return FromResult(result);
    }
}
