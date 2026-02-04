using Application.Services.Clients.DTOs;
using Application.Services.Clients;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Application.Common;

namespace WebAPI.Controllers.v1;

[Route("api/brokers/clients")]
public sealed class ClientsController(IClientService ClientService) : ApiController
{

    [HttpGet]
    public async Task<ActionResult<SearchClientsResponse>> SearchClient(
        [FromQuery] string? name,
        [FromQuery] string? identifier,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var pagedRequest = new PageRequest(page, pageSize);
        var result = await ClientService.SearchClientsAsync(
            new SearchClientsRequest(name, identifier, pagedRequest), ct);

        return FromResult(result);
    }

    [HttpGet("{clientId:guid}")]
    public async Task<ActionResult<GetClientDetailsResponse>> GetClientDetails(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await ClientService.GetClientDetailsAsync(
            new GetClientDetailsRequest(clientId), ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateClientResponse>> CreateClient(
        [FromBody] CreateClientRequest request,
        CancellationToken ct)
    {
        var result = await ClientService.CreateClientAsync(request, ct);

        return FromCreated(
            result,
            $"/api/brokers/clients/{result.Value!.ClientId}");
    }

    [HttpPut("{clientId:guid}")]
    public async Task<ActionResult<UpdateClientResponse>> UpdateClient(
        [FromBody] UpdateClientRequest request,
        CancellationToken ct)
    {
        var result = await ClientService.UpdateClientAsync(
            request, ct);

        return FromResult(result);
    }
}