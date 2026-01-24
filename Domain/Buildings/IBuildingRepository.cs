using Domain.Buildings;

namespace Infrastructure.Persistence.Repositories
{
    public interface IBuildingRepository
    {
        void Add(Building building);
        IReadOnlyList<Building> GetByClientId(Guid clientId);
        Building? GetById(Guid buildingId);
    }
}