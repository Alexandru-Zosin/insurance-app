using Application.Services.Shared.DTOs.Details;
using Application.Services.Shared.DTOs.Policy;

namespace Application.Services.Policies.DTOs;

public sealed record ListPoliciesResponse(ListResponse<PolicyListItemDto> Data);