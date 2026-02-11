using Domain.Buildings;
using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class BuildingTypeRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskBuildingType";

    public bool CanMapToDomain(PremiumRule premiumRuleRow)
        => string.Equals(premiumRuleRow.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule premiumRuleRow)
    {
        if (string.IsNullOrWhiteSpace(premiumRuleRow.BuildingType))
            throw new InvalidOperationException("RiskBuildingType requires BuildingType.");

        var rowBuildingType = Enum.Parse<BuildingType>(premiumRuleRow.BuildingType, ignoreCase: true);

        return BuildingTypeRiskConfiguration.FromState(
            id: premiumRuleRow.PremiumRuleKey,
            name: premiumRuleRow.Name,
            percentage: premiumRuleRow.Percentage,
            isActive: premiumRuleRow.IsActive,
            buildingType: rowBuildingType);
    }

    public bool CanMapToEf(IRiskConfiguration riskConfiguration)
        => riskConfiguration is BuildingTypeRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration riskConfiguration)
    {
        var buildingTypeRiskConfiguration = (BuildingTypeRiskConfiguration)riskConfiguration;

        return new PremiumRule
        {
            PremiumRuleKey = buildingTypeRiskConfiguration.Id,
            RuleKind = Kind,
            Name = buildingTypeRiskConfiguration.Name,
            Percentage = buildingTypeRiskConfiguration.Percentage,
            IsActive = buildingTypeRiskConfiguration.IsActive,
            BuildingType = buildingTypeRiskConfiguration.BuildingType.ToString()
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration riskConfiguration)
    {
        var buildingTypeRiskConfiguration = (BuildingTypeRiskConfiguration)riskConfiguration;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = buildingTypeRiskConfiguration.Name;
        row.Percentage = buildingTypeRiskConfiguration.Percentage;
        row.IsActive = buildingTypeRiskConfiguration.IsActive;
        row.BuildingType = buildingTypeRiskConfiguration.BuildingType.ToString();
    }
}
