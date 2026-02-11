using Application.Repositories;
using Domain.Buildings;
using Domain.Configurations;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfBuilding = Infrastructure.Persistence.Models.Building;
using EfRiskCategory = Infrastructure.Persistence.Models.RiskCategory;

namespace Infrastructure.Persistence.Repositories;

public sealed class BuildingRepository(InsuranceDbContext dbContext) : IBuildingRepository
{
    public async Task AddAsync(Building buildingToAdd, CancellationToken ct = default)
    {
        var buildingRow = await MapToEfAsync(buildingToAdd, ct);
        dbContext.Buildings.Add(buildingRow);
    }

    public async Task<IReadOnlyList<Building>> GetByClientIdAsync(Guid ownerClientId, CancellationToken ct = default)
    {
        var buildingRows = await dbContext.Buildings
            .AsNoTracking()
            .Include(b => b.RiskCategories)
            .Where(b => b.OwnerClientId == ownerClientId)
            .ToListAsync(ct);

        return buildingRows.Select(MapToDomain).ToList();
    }

    public async Task<Building?> GetByIdAsync(Guid buildingId, CancellationToken ct = default)
    {
        var buildingRow = await dbContext.Buildings
            .AsNoTracking()
            .Include(b => b.RiskCategories)
            .SingleOrDefaultAsync(b => b.BuildingKey == buildingId, ct);

        return buildingRow is null ? null : MapToDomain(buildingRow);
    }

    public async Task UpdateAsync(Building updatedBuilding, CancellationToken ct = default)
    {
        var existingBuildingRow = await dbContext.Buildings
            .Include(b => b.RiskCategories)
            .SingleOrDefaultAsync(b => b.BuildingKey == updatedBuilding.Id, ct);

        if (existingBuildingRow is null)
            throw new InvalidOperationException("Building not found.");

        await MapOntoEfAsync(existingBuildingRow, updatedBuilding, ct);
    }

    private async Task<EfBuilding> MapToEfAsync(Building building, CancellationToken ct)
    {
        var buildingRow = new EfBuilding
        {
            BuildingKey = building.Id,
            OwnerClientId = building.OwnerClientId,
            CityId = building.CityId,
            Street = building.Address.Street,
            Number = building.Address.Number,
            ConstructionYear = building.ConstructionYear,
            BuildingType = building.BuildingType.ToString(),
            SurfaceArea = building.SurfaceArea,
            InsuredValueAmount = building.InsuredValue.Amount,
            InsuredValueCurrencyCode = building.InsuredValue.CurrencyCode
        };
        await ApplyRiskCategoriesAsync(buildingRow, building, ct);
        
        return buildingRow;
    }

    private static Building MapToDomain(EfBuilding buildingRow)
    {
        var buildingAddress = Address.Create(buildingRow.Street, buildingRow.Number);
        var insuredValue = Money.Create(buildingRow.InsuredValueAmount, buildingRow.InsuredValueCurrencyCode);

        return Building.Rehydrate(
            id: buildingRow.BuildingKey,
            ownerClientId: buildingRow.OwnerClientId,
            address: buildingAddress,
            cityId: buildingRow.CityId,
            constructionYear: buildingRow.ConstructionYear,
            type: ParseBuildingType(buildingRow.BuildingType),
            surfaceArea: buildingRow.SurfaceArea,
            insuredValue: insuredValue,
            zoneRiskCategories: buildingRow.RiskCategories.Select(rc => ParseRiskCategory(rc.Code)).ToList());
    }

    private async Task MapOntoEfAsync(EfBuilding buildingRow, Building building, CancellationToken ct)
    {
        buildingRow.OwnerClientId = building.OwnerClientId;
        buildingRow.CityId = building.CityId;
        buildingRow.Street = building.Address.Street;
        buildingRow.Number = building.Address.Number;
        buildingRow.SurfaceArea = building.SurfaceArea;
        buildingRow.InsuredValueAmount = building.InsuredValue.Amount;
        buildingRow.InsuredValueCurrencyCode = building.InsuredValue.CurrencyCode;

        await ApplyRiskCategoriesAsync(buildingRow, building, ct);
    }

    private async Task ApplyRiskCategoriesAsync(EfBuilding buildingRow, Building domainBuilding, CancellationToken ct)
    {
        var riskCategoryCodes = domainBuilding.ZoneRiskCategories
            .Select(t => t.ToString())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var riskCategoryRows = (riskCategoryCodes.Count == 0) ? new List<EfRiskCategory>()
                : await dbContext.RiskCategories
                .Where(rc => riskCategoryCodes.Contains(rc.Code))
                .ToListAsync(ct);

        buildingRow.RiskCategories.Clear();
        foreach (var riskCategoryRow in riskCategoryRows)
            buildingRow.RiskCategories.Add(riskCategoryRow);
    }

    private static BuildingType ParseBuildingType(string buildingTypeValue)
    {
        if (Enum.TryParse<BuildingType>(buildingTypeValue, ignoreCase: true, out var buildingType))
            return buildingType;

        throw new InvalidOperationException($"Unknown building type '{buildingTypeValue}'.");
    }

    private static ZoneRiskCategory ParseRiskCategory(string riskCategoryCode)
    {
        if (Enum.TryParse<ZoneRiskCategory>(riskCategoryCode, ignoreCase: true, out var riskCategory))
            return riskCategory;

        throw new InvalidOperationException($"Unknown risk category code '{riskCategoryCode}'.");
    }
}
