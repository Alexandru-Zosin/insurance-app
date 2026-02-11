using Domain.Buildings;
namespace Application.Repositories;

public interface IBuildingRepository
{
    Task AddAsync(Building building, CancellationToken ct = default);
    Task<IReadOnlyList<Building>> ListByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task<Building?> GetByIdAsync(Guid buildingId, CancellationToken ct = default);
    Task UpdateAsync(Building building, CancellationToken ct = default);
}
