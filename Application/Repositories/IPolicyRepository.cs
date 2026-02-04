namespace Application.Repositories;

using Application.Common;
using Application.Services.Policies.DTOs;
using Domain.Policies;

public interface IPolicyRepository
{
    Task AddAsync(Policy policy, CancellationToken cancellationToken);
    Task UpdateAsync(Policy aggregate, CancellationToken ct = default);
    Task<Policy?> GetByIdAsync(Guid policyId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Policy>> GetByClientIdAsync(Guid clientId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Policy>> GetByBuildingIdAsync(Guid buildingId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Policy>> SearchAsync(PolicySearchCriteria criteria,
                                            PageRequest page,
                                            CancellationToken ct = default);
}
