namespace Application.Repositories;

using Application.Common;
using Application.Repositories.SearchCriteria;
using Domain.Policies;

public interface IPolicyRepository
{
    void Add(Policy policy);
    Task UpdateAsync(Policy policy, CancellationToken ct = default);
    Task<Policy?> GetByIdAsync(Guid policyId, CancellationToken ct = default);
    Task<IReadOnlyList<Policy>> ListAsync(PolicySearchCriteria criteria, PageRequest? pageRequest = null,
                                            CancellationToken ct = default);
}