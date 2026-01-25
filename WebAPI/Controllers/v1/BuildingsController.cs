using Application.UseCases.Buildings;
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
    public async Task<ActionResult<GetBuildingsForClientService.Response>> GetForClient(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await _getForClient.HandleAsync(
            new GetBuildingsForClientService.Request(clientId), ct);

        return FromResult(result);
    }

    [HttpGet("buildings/{buildingId:guid}")]
    public async Task<ActionResult<GetBuildingDetailsService.Response>> Get(
        Guid buildingId,
        CancellationToken ct)
    {
        var result = await _getDetails.HandleAsync(
            new GetBuildingDetailsService.Request(buildingId), ct);

        return FromResult(result);
    }

    [HttpPost("clients/{clientId:guid}/buildings")]
    public async Task<ActionResult<RegisterBuildingService.Response>> Register(
        Guid clientId,
        [FromBody] RegisterBuildingService.Request request,
        CancellationToken ct)
    {
        var result = await _register.HandleAsync(
            request with { ClientId = clientId }, ct);

        return FromCreated(
            result,
            $"/api/brokers/buildings/{result.Value!.BuildingId}");
    }

    [HttpPut("buildings/{buildingId:guid}")]
    public async Task<ActionResult<UpdateBuildingService.Response>> Update(
        Guid buildingId,
        [FromBody] UpdateBuildingService.Request request,
        CancellationToken ct)
    {
        var result = await _update.HandleAsync(
            request with { BuildingId = buildingId }, ct);

        return FromResult(result);
    }
}
