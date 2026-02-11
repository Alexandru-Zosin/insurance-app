using Application.Common;
using Application.Services.Policies;
using Application.Services.Policies.DTOs;
using Domain.Policies;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class PoliciesController(IPolicyService policyService) : ApiController 
{
    [HttpGet("policies")]
    public async Task<ActionResult<ListPoliciesResponse>> ListPoliciesAsync(
        [FromQuery] Guid? clientId,
        [FromQuery] Guid? brokerId,
        [FromQuery] PolicyStatus? status,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default
    )
    {
        var pagedRequest = new PageRequest(page, pageSize);
        var result = await policyService.ListPoliciesAsync(
            new ListPoliciesRequest(clientId, brokerId, status, startDate, endDate, pagedRequest), ct);

        return FromResult(result);
    }

    [HttpGet("policies/{policyNumber:guid}")]
    public async Task<ActionResult<GetPolicyDetailsResponse>> GetPolicyDetailsAsync(
        [FromRoute] Guid policyNumber,
        CancellationToken ct = default)
    {
        var result = await policyService.GetPolicyDetailsAsync(policyNumber, ct);

        return FromResult(result);
    }

    [HttpPost("policies")]
    public async Task<ActionResult<CreateDraftPolicyResponse>> CreateDraftPolicyAsync(
        [FromBody] CreateDraftPolicyRequest request,
        CancellationToken ct = default)
    {
        var result = await policyService.CreateDraftPolicyAsync(request, ct);

        return FromCreated(result, $"/api/brokers/policies/{result.Value!.Policy.Id}");
    }

    [HttpPost("policies/{policyNumber:guid}/activate")]
    public async Task<ActionResult<ActivatePolicyResponse>> ActivatePolicyAsync(
        [FromRoute] Guid policyNumber,
        CancellationToken ct = default)
    {
        var result = await policyService.ActivatePolicyAsync(policyNumber, ct);

        return FromResult(result);
    }

    [HttpPost("policies/{policyNumber:guid}/cancel")]
    public async Task<ActionResult<CancelPolicyResponse>> CancelPolicyAsync(
        [FromRoute] Guid policyNumber,
        [FromBody] CancelPolicyRequest request,
        CancellationToken ct = default)
    {
        var result = await policyService.CancelPolicyAsync(policyNumber, request, ct);

        return FromResult(result);
    }
}
