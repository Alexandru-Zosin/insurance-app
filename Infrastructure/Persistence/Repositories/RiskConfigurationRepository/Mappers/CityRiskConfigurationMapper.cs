using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class CityRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskCity";

    public bool CanMaterialize(PremiumRule row)
        => string.Equals(row.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration ToDomain(PremiumRule row)
    {
        if (row.CityId is null)
            throw new InvalidOperationException("RiskCity requires CityId.");

        return CityRiskConfiguration.Rehydrate(
            id: row.PremiumRuleKey,
            name: row.Name,
            percentage: row.Percentage,
            isActive: row.IsActive,
            cityId: row.CityId.Value);
    }

    public bool CanPersist(IRiskConfiguration aggregate)
        => aggregate is CityRiskConfiguration;

    public PremiumRule ToEfModel(IRiskConfiguration aggregate)
    {
        var a = (CityRiskConfiguration)aggregate;

        return new PremiumRule
        {
            PremiumRuleKey = a.Id,
            RuleKind = Kind,
            Name = a.Name,
            Percentage = a.Percentage,
            IsActive = a.IsActive,
            CityId = a.CityId
        };
    }

    public void UpdateEfModel(PremiumRule row, IRiskConfiguration aggregate)
    {
        var a = (CityRiskConfiguration)aggregate;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = a.Name;
        row.Percentage = a.Percentage;
        row.IsActive = a.IsActive;
        row.CityId = a.CityId;
    }
}
