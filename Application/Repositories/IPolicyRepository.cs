namespace Application.Repositories;

using Application.Common;
using Application.Services.Policies.DTOs;
using Domain.Policies;

public interface IPolicyRepository
{
    void Add(Policy policy, CancellationToken ct = default);
    Task UpdateAsync(Policy policy, CancellationToken ct = default);
    Task<Policy?> GetByIdAsync(Guid policyId, CancellationToken ct = default);
    Task<IReadOnlyList<Policy>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task<IReadOnlyList<Policy>> GetByBuildingIdAsync(Guid buildingId, CancellationToken ct = default);
    Task<IReadOnlyList<Policy>> SearchAsync(PolicySearchCriteria criteria, PageRequest pageRequest,
                                            CancellationToken ct = default);
}
