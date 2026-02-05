using Application.Common;
using Application.Services.Policies;
using Application.Services.Policies.DTOs;
using Domain.Policies;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

namespace WebAPI.Controllers;

[Route("api/brokers")]
public sealed class PoliciesController(IPolicyService PolicyService) : ApiController 
{
    [HttpGet("policies")]
    public async Task<ActionResult<ListPoliciesResponse>> List(
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
        var result = await PolicyService.ListPoliciesAsync(
            new ListPoliciesRequest(clientId, brokerId, status, startDate, endDate, pagedRequest), ct);

        return FromResult(result);
    }


    [HttpGet("policies/{policyId:guid}")]
    public async Task<ActionResult<GetPolicyDetailsResponse>> Get(
        Guid policyId,
        CancellationToken ct)
    {
        var result = await PolicyService.GetPolicyDetailsAsync(
            new GetPolicyDetailsRequest(policyId), ct);

        return FromResult(result);
    }

    [HttpPost("policies")]
    public async Task<ActionResult<CreateDraftPolicyResponse>> CreateDraft(
        [FromBody] CreateDraftPolicyRequest request,
        CancellationToken ct)
    {
        var result = await PolicyService.CreateDraftPolicyAsync(request, ct);

        return FromCreated(
            result,
            $"/api/brokers/policies/{result.Value!.PolicyId}");
    }

    [HttpPost("policies/{policyId:guid}/activate")]
    public async Task<ActionResult<ActivatePolicyResponse>> Activate(
        [FromBody] ActivatePolicyRequest request,
        CancellationToken ct)
    {
        var result = await PolicyService.ActivatePolicyAsync(request, ct);

        return FromResult(result);
    }

    [HttpPost("policies/{policyId:guid}/cancel")]
    public async Task<ActionResult<CancelPolicyResponse>> Cancel(
        [FromBody] CancelPolicyRequest request,
        CancellationToken ct)
    {
        var result = await PolicyService.CancelPolicyAsync(request, ct);

        return FromResult(result);
    }
}
