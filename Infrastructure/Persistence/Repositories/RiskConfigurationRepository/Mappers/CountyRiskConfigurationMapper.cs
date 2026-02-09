using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class CountyRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "CountyRisk";

    public bool CanMap(PremiumRule row)
        => string.Equals(row.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule row)
    {
        if (row.CountyId is null)
            throw new InvalidOperationException("CountyRisk requires CountyId.");

        return CountyRiskConfiguration.Rehydrate(
            id: row.PremiumRuleKey,
            name: row.Name,
            percentage: row.Percentage,
            isActive: row.IsActive,
            countyId: row.CountyId.Value);
    }

    public bool CanPersist(IRiskConfiguration aggregate)
        => aggregate is CountyRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration aggregate)
    {
        var a = (CountyRiskConfiguration)aggregate;

        return new PremiumRule
        {
            PremiumRuleKey = a.Id,
            RuleKind = Kind,
            Name = a.Name,
            Percentage = a.Percentage,
            IsActive = a.IsActive,
            CountyId = a.CountyId,
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration aggregate)
    {
        var a = (CountyRiskConfiguration)aggregate;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = a.Name;
        row.Percentage = a.Percentage;
        row.IsActive = a.IsActive;
        row.CountyId = a.CountyId;
    }
}
