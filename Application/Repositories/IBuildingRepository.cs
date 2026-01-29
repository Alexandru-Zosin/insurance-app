using Domain.Buildings;
namespace Application.Repositories;

public interface IBuildingRepository
{
    Task AddAsync(Building building, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Building>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<Building?> GetByIdAsync(Guid buildingId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Building building, CancellationToken cancellationToken = default);
}
