using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class CountyRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "CountyRisk";

    public bool CanMapToDomain(PremiumRule premiumRuleRow)
        => string.Equals(premiumRuleRow.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule premiumRuleRow)
    {
        if (premiumRuleRow.CountyId is null)
            throw new InvalidOperationException("CountyRisk requires CountyId.");

        var premiumRuleRowCountyId = premiumRuleRow.CountyId.Value;

        return CountyRiskConfiguration.FromState(
            id: premiumRuleRow.PremiumRuleKey,
            name: premiumRuleRow.Name,
            percentage: premiumRuleRow.Percentage,
            isActive: premiumRuleRow.IsActive,
            countyId: premiumRuleRowCountyId);
    }

    public bool CanMapToEf(IRiskConfiguration riskConfiguration)
        => riskConfiguration is CountyRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration riskConfiguration)
    {
        var countyRiskConfiguration = (CountyRiskConfiguration)riskConfiguration;

        return new PremiumRule
        {
            PremiumRuleKey = countyRiskConfiguration.Id,
            RuleKind = Kind,
            Name = countyRiskConfiguration.Name,
            Percentage = countyRiskConfiguration.Percentage,
            IsActive = countyRiskConfiguration.IsActive,
            CountyId = countyRiskConfiguration.CountyId
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration riskConfiguration)
    {
        var countyRiskConfiguration = (CountyRiskConfiguration)riskConfiguration;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = countyRiskConfiguration.Name;
        row.Percentage = countyRiskConfiguration.Percentage;
        row.IsActive = countyRiskConfiguration.IsActive;
        row.CountyId = countyRiskConfiguration.CountyId;
    }
}
