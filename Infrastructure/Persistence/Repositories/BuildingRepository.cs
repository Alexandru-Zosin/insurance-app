using Application.Repositories;
using Domain.Buildings;
using Domain.Configurations;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfBuilding = Infrastructure.Persistence.Models.Building;

namespace Infrastructure.Persistence.Repositories;

public sealed class BuildingRepository(InsuranceDbContext _db) : IBuildingRepository
{
    public async Task AddAsync(Building building, CancellationToken cancellationToken = default)
    {
        if (building is null) throw new ArgumentNullException(nameof(building));

        var ef = await ToEfModelAsync(building, cancellationToken).ConfigureAwait(false);

        _db.Buildings.Add(ef);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Building>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        if (clientId == Guid.Empty) throw new ArgumentException("Client id is required.", nameof(clientId));

        var rows = await _db.Buildings
            .AsNoTracking()
            .Include(b => b.RiskCategories)
            .Where(b => b.OwnerClientId == clientId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    public async Task<Building?> GetByIdAsync(Guid buildingId, CancellationToken cancellationToken = default)
    {
        if (buildingId == Guid.Empty) throw new ArgumentException("Building id is required.", nameof(buildingId));

        var row = await _db.Buildings
            .AsNoTracking()
            .Include(b => b.RiskCategories)
            .SingleOrDefaultAsync(b => b.BuildingKey == buildingId, cancellationToken)
            .ConfigureAwait(false);

        return row is null ? null : ToDomain(row);
    }

    public async Task UpdateAsync(Building building, CancellationToken cancellationToken = default)
    {
        if (building is null) throw new ArgumentNullException(nameof(building));

        var ef = await _db.Buildings
            .Include(b => b.RiskCategories)
            .SingleOrDefaultAsync(b => b.BuildingKey == building.Id, cancellationToken)
            .ConfigureAwait(false);

        if (ef is null)
            throw new InvalidOperationException("Building not found.");

        await UpdateEfModelAsync(ef, building, cancellationToken).ConfigureAwait(false);

        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<EfBuilding> ToEfModelAsync(Building domain, CancellationToken ct)
    {
        var ef = new EfBuilding
        {
            BuildingKey = domain.Id,
            OwnerClientId = domain.OwnerClientId,
            CityId = domain.CityId,
            Street = domain.Address.Street,
            Number = domain.Address.Number,
            ConstructionYear = domain.ConstructionYear,
            BuildingType = domain.BuildingType.ToString(),
            SurfaceArea = domain.SurfaceArea,
            InsuredValueAmount = domain.InsuredValue.Amount,
            InsuredValueCurrencyCode = domain.InsuredValue.CurrencyCode
        };

        await SetRiskCategoriesAsync(ef, domain, ct).ConfigureAwait(false);
        return ef;
    }

    private static Building ToDomain(EfBuilding ef)
    {
        var address = Address.Create(ef.Street, ef.Number);
        var insuredValue = Money.Create(ef.InsuredValueAmount, ef.InsuredValueCurrencyCode);

        return Building.Rehydrate(
            id: ef.BuildingKey,
            ownerClientId: ef.OwnerClientId,
            address: address,
            cityId: ef.CityId,
            constructionYear: ef.ConstructionYear,
            type: ParseBuildingType(ef.BuildingType),
            surfaceArea: ef.SurfaceArea,
            insuredValue: insuredValue,
            zoneRiskCategories: ef.RiskCategories.Select(rc => ParseRiskCategory(rc.Code)).ToList());
    }

    private async Task UpdateEfModelAsync(EfBuilding ef, Building domain, CancellationToken ct)
    {
        ef.OwnerClientId = domain.OwnerClientId;
        ef.CityId = domain.CityId;
        ef.Street = domain.Address.Street;
        ef.Number = domain.Address.Number;
        ef.SurfaceArea = domain.SurfaceArea;
        ef.InsuredValueAmount = domain.InsuredValue.Amount;
        ef.InsuredValueCurrencyCode = domain.InsuredValue.CurrencyCode;

        await SetRiskCategoriesAsync(ef, domain, ct).ConfigureAwait(false);
    }

    private async Task SetRiskCategoriesAsync(EfBuilding ef, Building domain, CancellationToken ct)
    {
        var codes = domain.ZoneRiskCategories
            .Select(t => t.ToString())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var risks = codes.Count == 0
            ? new List<Infrastructure.Persistence.Models.RiskCategory>()
            : await _db.RiskCategories
                .Where(rc => codes.Contains(rc.Code))
                .ToListAsync(ct)
                .ConfigureAwait(false);

        ef.RiskCategories.Clear();
        foreach (var r in risks)
            ef.RiskCategories.Add(r);
    }

    private static BuildingType ParseBuildingType(string value)
    {
        if (Enum.TryParse<BuildingType>(value, ignoreCase: true, out var parsed))
            return parsed;

        throw new InvalidOperationException($"Unknown building type '{value}'.");
    }

    private static ZoneRiskCategory ParseRiskCategory(string code)
    {
        if (Enum.TryParse<ZoneRiskCategory>(code, ignoreCase: true, out var parsed))
            return parsed;

        throw new InvalidOperationException($"Unknown risk category code '{code}'.");
    }
}
