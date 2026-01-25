using Application.Services.Clients.DTO;
using Application.UseCases.Clients;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[Route("api/brokers/clients")]
public sealed class ClientsController : BaseApiController
{
    private readonly SearchClientsService _search;
    private readonly GetClientDetailsService _getDetails;
    private readonly CreateClientService _create;
    private readonly UpdateClientService _update;

    public ClientsController(
        SearchClientsService search,
        GetClientDetailsService getDetails,
        CreateClientService create,
        UpdateClientService update)
    {
        _search = search;
        _getDetails = getDetails;
        _create = create;
        _update = update;
    }

    [HttpGet]
    public async Task<ActionResult<SearchClientsResponse>> Search(
        [FromQuery] string? name,
        [FromQuery] string? identifier,
        CancellationToken ct)
    {
        var result = await _search.HandleAsync(
            new SearchClientsRequest(name, identifier), ct);

        return FromResult(result);
    }

    [HttpGet("{clientId:guid}")]
    public async Task<ActionResult<GetClientDetailsResponse>> Get(
        Guid clientId,
        CancellationToken ct)
    {
        var result = await _getDetails.HandleAsync(
            new GetClientDetailsRequest(clientId), ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateClientResponse>> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken ct)
    {
        var result = await _create.HandleAsync(request, ct);

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
        var result = await _update.HandleAsync(
            request with { ClientId = clientId }, ct);

        return FromResult(result);
    }
}
