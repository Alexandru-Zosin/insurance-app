using Application.Services.Shared.DTOs.PolicyDTOs;
namespace Application.Services.Policies.DTOs;

public sealed record ListPoliciesResponse(IReadOnlyList<PolicyListItemDto> Policies);