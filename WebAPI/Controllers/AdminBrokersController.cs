using Application.Common;
using Application.Services.Brokers;
using Application.Services.Brokers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/brokers")]
public sealed class AdminBrokersController(IBrokerService BrokerService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListBrokersResponse>> ListBrokersAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var pagedRequest = new PageRequest(pageNumber, pageSize);
        var result = await BrokerService.ListBrokersAsync(
            new ListBrokersRequest(pagedRequest), ct);

        return FromResult(result);
    }

    [HttpGet("{brokerId:guid}")]
    public async Task<ActionResult<GetBrokerDetailsResponse>> GetBrokerDetailsAsync(
        Guid brokerId,
        CancellationToken ct)
    {
        var result = await BrokerService.GetBrokerDetailsAsync(brokerId, ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateBrokerResponse>> CreateBrokerAsync(
        [FromBody] CreateBrokerRequest request,
        CancellationToken ct)
    {
        var result = await BrokerService.CreateBrokerAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/brokers/{result.Value!.Broker.Id}");
    }

    [HttpPut("{brokerId:guid}")]
    public async Task<ActionResult<UpdateBrokerResponse>> UpdateBrokerAsync(
        [FromRoute] Guid brokerId,
        [FromBody] UpdateBrokerRequest request,
        CancellationToken ct)
    {
        var result = await BrokerService.UpdateBrokerAsync(brokerId, request, ct);

        return FromResult(result);
    }

    [HttpPost("{brokerId:guid}/activate")]
    public async Task<ActionResult<SetBrokerStatusResponse>> ActivateBrokerAsync(
        [FromRoute] Guid brokerId,
        CancellationToken ct)
    {
        var result = await BrokerService.SetBrokerStatusAsync(brokerId, true, ct);

        return FromResult(result);
    }

    [HttpPost("{brokerId:guid}/deactivate")]
    public async Task<ActionResult<SetBrokerStatusResponse>> DeactivateBrokerAsync(
        [FromRoute] Guid brokerId,
        CancellationToken ct)
    {
        var result = await BrokerService.SetBrokerStatusAsync(brokerId, false, ct);

        return FromResult(result);
    }
}
