namespace Application.Repositories;
using Domain.Policies;

public interface IPolicyRepository
{
    Task<Policy?> GetByIdAsync(Guid policyId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Policy>> GetByClientIdAsync(Guid clientId,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Policy>> GetByBuildingIdAsync(Guid buildingId,
        CancellationToken cancellationToken);
    Task AddAsync(Policy policy,
        CancellationToken cancellationToken);
}
