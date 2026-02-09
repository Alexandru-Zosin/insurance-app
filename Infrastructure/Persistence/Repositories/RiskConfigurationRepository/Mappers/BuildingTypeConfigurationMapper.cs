using Domain.Configurations;
using Domain.Buildings;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class BuildingTypeRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskBuildingType";

    public bool CanMap(PremiumRule row)
        => string.Equals(row.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule row)
    {
        if (string.IsNullOrWhiteSpace(row.BuildingType))
            throw new InvalidOperationException("RiskBuildingType requires BuildingType.");

        var bt = Enum.Parse<BuildingType>(row.BuildingType, ignoreCase: true);

        return BuildingTypeRiskConfiguration.Rehydrate(
            id: row.PremiumRuleKey,
            name: row.Name,
            percentage: row.Percentage,
            isActive: row.IsActive,
            buildingType: bt);
    }

    public bool CanPersist(IRiskConfiguration aggregate)
        => aggregate is BuildingTypeRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration aggregate)
    {
        var a = (BuildingTypeRiskConfiguration)aggregate;

        return new PremiumRule
        {
            PremiumRuleKey = a.Id,
            RuleKind = Kind,
            Name = a.Name,
            Percentage = a.Percentage,
            IsActive = a.IsActive,
            BuildingType = a.BuildingType.ToString()
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration aggregate)
    {
        var a = (BuildingTypeRiskConfiguration)aggregate;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = a.Name;
        row.Percentage = a.Percentage;
        row.IsActive = a.IsActive;
        row.BuildingType = a.BuildingType.ToString();
    }
}
