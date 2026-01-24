namespace Domain.Buildings;

public interface IBuildingRepository
{
    Building? GetById(Guid buildingId);
    IReadOnlyList<Building> GetByClientId(Guid clientId);
    void Add(Building building);
}
