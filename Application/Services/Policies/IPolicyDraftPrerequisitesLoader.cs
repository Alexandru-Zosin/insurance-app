using Application.Common;
using Application.Services.Policies.DTOs;

namespace Application.Services.Policies
{
    public interface IPolicyDraftPrerequisitesLoader
    {
        Task<Result<PolicyDraftPrerequisites>> LoadAsync(CreateDraftPolicyRequest request, CancellationToken ct = default);
    }
}