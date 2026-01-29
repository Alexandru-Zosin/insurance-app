using Application.Services.Clients.DTOs;
using Application.Services.Clients;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[Route("api/brokers/clients")]
public sealed class ClientsController(IClientService _clientService) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<SearchClientsResponse>> Search(
        [FromQuery] string? name,
        [FromQuery] string? identifier,
        CancellationToken ct)
    {
        var result = await _clientService.SearchClientsAsync(
            new SearchClientsRequest(name, identifier), ct);

        return FromResult(result);
    }

    [HttpGet("{clientId:guid}")]
    public async Task<ActionResult<GetClientDetailsResponse>> Get(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await _clientService.GetClientDetailsAsync(
            new GetClientDetailsRequest(clientId), ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateClientResponse>> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken ct)
    {
        var result = await _clientService.CreateClientAsync(request, ct);

        return FromCreated(
            result,
            $"/api/brokers/clients/{result.Value!.ClientId}");
    }

    [HttpPut("{clientId:guid}")]
    public async Task<ActionResult<UpdateClientResponse>> Update(
        Guid clientId,
        [FromBody] UpdateClientRequest request,
        CancellationToken ct)
    {
        var result = await _clientService.UpdateClientAsync(
            request with { ClientId = clientId }, ct);

        return FromResult(result);
    }
}
