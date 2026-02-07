using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class ZoneRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskZoneCategory";

    public bool CanMaterialize(PremiumRule row)
        => string.Equals(row.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration ToDomain(PremiumRule row)
    {
        if (string.IsNullOrWhiteSpace(row.ZoneRiskCategoryCode))
            throw new InvalidOperationException("RiskZoneCategory requires ZoneRiskCategoryCode.");

        var category = Enum.Parse<ZoneRiskCategory>(row.ZoneRiskCategoryCode, ignoreCase: true);

        return ZoneRiskConfiguration.Rehydrate(
            id: row.PremiumRuleKey,
            name: row.Name,
            pct: row.Percentage,
            active: row.IsActive,
            category: category);
    }

    public bool CanPersist(IRiskConfiguration aggregate)
        => aggregate is ZoneRiskConfiguration;

    public PremiumRule ToEfModel(IRiskConfiguration aggregate)
    {
        var a = (ZoneRiskConfiguration)aggregate;

        return new PremiumRule
        {
            PremiumRuleKey = a.Id,
            RuleKind = Kind,
            Name = a.Name,
            Percentage = a.Percentage,
            IsActive = a.IsActive,
            ZoneRiskCategoryCode = a.Category.ToString()
        };
    }

    public void UpdateEfModel(PremiumRule row, IRiskConfiguration aggregate)
    {
        var a = (ZoneRiskConfiguration)aggregate;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = a.Name;
        row.Percentage = a.Percentage;
        row.IsActive = a.IsActive;
        row.ZoneRiskCategoryCode = a.Category.ToString();
    }
}
