using Application.Common;
using Application.Services.Policies.DTOs;

namespace Application.Services.Policies
{
    public interface IPolicyService
    {
        Task<Result<ActivatePolicyResponse>> ActivatePolicyAsync(Guid policyNumber, ActivatePolicyRequest request, CancellationToken ct = default);
        Task<Result<CancelPolicyResponse>> CancelPolicyAsync(Guid policyNumber, CancelPolicyRequest request, CancellationToken ct = default);
        Task<Result<CreateDraftPolicyResponse>> CreateDraftPolicyAsync(CreateDraftPolicyRequest request, CancellationToken ct = default);
        Task<Result<GetPolicyDetailsResponse>> GetPolicyDetailsAsync(GetPolicyDetailsRequest request, CancellationToken ct = default);
        Task<Result<ListPoliciesResponse>> ListPoliciesAsync(ListPoliciesRequest request, CancellationToken ct = default);
    }
}