using Domain.Buildings;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class BuildingRepository : IBuildingRepository
{
    private readonly InsuranceDbContext _db;

    public BuildingRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public Domain.Buildings.Building? GetById(Guid buildingId)
    {
        var ef = _db.Buildings
            .Include(b => b.City)
            .AsNoTracking()
            .SingleOrDefault(b => b.BuildingKey == buildingId);

        return ef == null ? null : Map(ef);
    }

    public IReadOnlyList<Domain.Buildings.Building> GetByClientId(Guid clientId)
    {
        return _db.Buildings
            .Include(b => b.City)
            .Where(b => b.Client.ClientKey == clientId)
            .AsNoTracking()
            .Select(Map)
            .ToList();
    }

    public void Add(Domain.Buildings.Building building)
    {
        var clientId = _db.Clients
                        .Where(c => c.ClientKey == building.ClientId)
                        .Select(c => c.ClientId)
                        .Single();

        _db.Buildings.Add(new Models.Building
        {
            BuildingKey = building.Id,
            ClientId = clientId,
            CityId = building.City.Id,
            ConstructionYear = building.ConstructionYear,
            Address = $"{building.Address.Street} {building.Address.Number}",
            BuildingType = building.BuildingType.ToString(),
            NumberOfFloors = 1,
            SurfaceArea = building.SurfaceArea,
            InsuredValue = (int)building.InsuredValue.Amount,
            InsuredValueCurrency = building.InsuredValue.Currency,
            FloodRiskZone = building.RiskProfile.FloodRisk ? 1 : 0,
            EarthquakeRiskZone = building.RiskProfile.EarthquakeRisk ? 1 : 0
        });
    }
    private static Domain.Buildings.Building Map(Models.Building ef)
    {
        var address = Address.Create(ef.Address, "").Value!;
        var money = Money.Create(ef.InsuredValue, ef.InsuredValueCurrency).Value!;
        var risk = new RiskProfile(
            ef.FloodRiskZone == 1,
            ef.EarthquakeRiskZone == 1);

        var city = new Domain.Geography.City(
            ef.City.CityId,
            ef.City.Name);

        return Domain.Buildings.Building.Create(
            ef.Client.ClientKey,
            address,
            city,
            ef.ConstructionYear,
            Enum.Parse<BuildingType>(ef.BuildingType),
            ef.SurfaceArea,
            money,
            risk).Value!;
    }
}
