using Application.Services.Shared.DTOs.PolicyDTOs;
namespace Application.Services.Policies.DTOs;

public sealed record CreateDraftPolicyRequest(PolicyCoreDto Policy);
