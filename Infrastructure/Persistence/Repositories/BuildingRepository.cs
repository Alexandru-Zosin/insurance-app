using Application.Repositories;
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

    public async Task<Domain.Buildings.Building?> GetByIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Buildings
            .Include(b => b.Client)
            .Include(b => b.City)
            .AsNoTracking()
            .SingleOrDefaultAsync(
                b => b.BuildingKey == buildingId,
                cancellationToken)
            .ConfigureAwait(false);

        return ef == null ? null : Map(ef);
    }

    public async Task<IReadOnlyList<Domain.Buildings.Building>> GetByClientIdAsync(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        var entities = await _db.Buildings
            .Include(b => b.Client)
            .Include(b => b.City)
            .Where(b => b.Client.ClientKey == clientId)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entities.Select(Map).ToList();
    }

    public async Task AddAsync(
        Domain.Buildings.Building building,
        CancellationToken cancellationToken)
    {
        var clientId = await _db.Clients
            .Where(c => c.ClientKey == building.ClientId)
            .Select(c => c.ClientId)
            .SingleAsync(cancellationToken)
            .ConfigureAwait(false);

        await _db.Buildings.AddAsync(
            new Models.Building
            {
                BuildingKey = building.Id,
                ClientId = clientId,
                CityId = building.City.Id,
                ConstructionYear = building.ConstructionYear,
                Street = building.Address.Street,
                Number = building.Address.Number,
//                Address = $"{building.Address.Street} {building.Address.Number}",
                BuildingType = building.BuildingType.ToString(),
                NumberOfFloors = 1,
                SurfaceArea = building.SurfaceArea,
                InsuredValue = (int)building.InsuredValue.Amount,
                InsuredValueCurrency = building.InsuredValue.Currency,
                FloodRiskZone = building.RiskProfile.FloodRisk ? 1 : 0,
                EarthquakeRiskZone = building.RiskProfile.EarthquakeRisk ? 1 : 0
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task UpdateAsync(
    Domain.Buildings.Building building,
    CancellationToken cancellationToken)
    {
        var ef = await _db.Buildings
            .SingleAsync(
                b => b.BuildingKey == building.Id,
                cancellationToken)
            .ConfigureAwait(false);

        ef.CityId = building.City.Id;
        ef.ConstructionYear = building.ConstructionYear;
        ef.Street = building.Address.Street;
        ef.Number = building.Address.Number;
//        ef.Address = $"{building.Address.Street} {building.Address.Number}";
        ef.BuildingType = building.BuildingType.ToString();
        ef.SurfaceArea = building.SurfaceArea;
        ef.InsuredValue = (int)building.InsuredValue.Amount;
        ef.InsuredValueCurrency = building.InsuredValue.Currency;
        ef.FloodRiskZone = building.RiskProfile.FloodRisk ? 1 : 0;
        ef.EarthquakeRiskZone = building.RiskProfile.EarthquakeRisk ? 1 : 0;
    }
    private static Domain.Buildings.Building Map(Models.Building ef)
    {
        var address = Address.Create(ef.Street, ef.Number).Value!;
        var money = Money.Create(ef.InsuredValue, ef.InsuredValueCurrency).Value!;
        var risk = new RiskProfile(
            ef.FloodRiskZone == 1,
            ef.EarthquakeRiskZone == 1);

        var city = new Domain.Geography.City(
            ef.City.CityId,
            ef.City.Name);

        return Domain.Buildings.Building.Rehydrate(
            ef.BuildingKey,
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
