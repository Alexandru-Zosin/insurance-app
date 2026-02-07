using Application.Common;
using Application.Services.Brokers;
using Application.Services.Brokers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/admin/brokers")]
public sealed class AdminBrokersController(IBrokerService BrokerService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<ListBrokersResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var pagedRequest = new PageRequest(page, pageSize);
        var result = await BrokerService.ListBrokersAsync(
            new ListBrokersRequest(pagedRequest), ct);

        return FromResult(result);
    }

    [HttpGet("{brokerId:guid}")]
    public async Task<ActionResult<GetBrokerDetailsResponse>> GetDetails(
        Guid brokerId,
        CancellationToken ct)
    {
        var result = await BrokerService.GetBrokerDetailsAsync(
            new GetBrokerDetailsRequest(brokerId), ct);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateBrokerResponse>> Create(
        [FromBody] CreateBrokerRequest request,
        CancellationToken ct)
    {
        var result = await BrokerService.CreateBrokerAsync(request, ct);

        return FromCreated(
            result,
            $"/api/admin/brokers/{result.Value!.Broker.Id}");
    }

    [HttpPut("{brokerId:guid}")]
    public async Task<ActionResult<UpdateBrokerResponse>> Update(
        [FromBody] UpdateBrokerRequest request,
        CancellationToken ct)
    {
        var result = await BrokerService.UpdateBrokerAsync(request, ct);

        return FromResult(result);
    }

    [HttpPost("{brokerId:guid}/status")]
    public async Task<ActionResult<SetBrokerStatusResponse>> SetStatus(
        [FromBody] SetBrokerStatusRequest request,
        CancellationToken ct)
    {
        var result = await BrokerService.SetBrokerStatusAsync(request, ct);

        return FromResult(result);
    }
}
