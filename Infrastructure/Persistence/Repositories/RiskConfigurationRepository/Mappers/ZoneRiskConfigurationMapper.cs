using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class ZoneRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskZoneCategory";

    public bool CanMapToDomain(PremiumRule premiumRuleRow)
        => string.Equals(premiumRuleRow.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule premiumRuleRow)
    {
        if (string.IsNullOrWhiteSpace(premiumRuleRow.ZoneRiskCategoryCode))
            throw new InvalidOperationException("RiskZoneCategory requires ZoneRiskCategoryCode.");

        var premiumRuleRowZoneRiskCategory = Enum.Parse<ZoneRiskCategory>(
            premiumRuleRow.ZoneRiskCategoryCode,
            ignoreCase: true);

        return ZoneRiskConfiguration.FromState(
            id: premiumRuleRow.PremiumRuleKey,
            name: premiumRuleRow.Name,
            pct: premiumRuleRow.Percentage,
            active: premiumRuleRow.IsActive,
            category: premiumRuleRowZoneRiskCategory);
    }

    public bool CanMapToEf(IRiskConfiguration riskConfiguration)
        => riskConfiguration is ZoneRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration riskConfiguration)
    {
        var zoneRiskConfiguration = (ZoneRiskConfiguration)riskConfiguration;

        return new PremiumRule
        {
            PremiumRuleKey = zoneRiskConfiguration.Id,
            RuleKind = Kind,
            Name = zoneRiskConfiguration.Name,
            Percentage = zoneRiskConfiguration.Percentage,
            IsActive = zoneRiskConfiguration.IsActive,
            ZoneRiskCategoryCode = zoneRiskConfiguration.Category.ToString()
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration riskConfiguration)
    {
        var zoneRiskConfiguration = (ZoneRiskConfiguration)riskConfiguration;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = zoneRiskConfiguration.Name;
        row.Percentage = zoneRiskConfiguration.Percentage;
        row.IsActive = zoneRiskConfiguration.IsActive;
        row.ZoneRiskCategoryCode = zoneRiskConfiguration.Category.ToString();
    }
}
