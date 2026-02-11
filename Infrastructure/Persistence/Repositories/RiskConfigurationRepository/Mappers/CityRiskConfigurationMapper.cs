using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class CityRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskCity";

    public bool CanMapToDomain(PremiumRule premiumRuleRow)
        => string.Equals(premiumRuleRow.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule premiumRuleRow)
    {
        if (premiumRuleRow.CityId is null)
            throw new InvalidOperationException("RiskCity requires CityId.");

        var premiumRuleRowCityId = premiumRuleRow.CityId.Value;

        return CityRiskConfiguration.FromState(
            id: premiumRuleRow.PremiumRuleKey,
            name: premiumRuleRow.Name,
            percentage: premiumRuleRow.Percentage,
            isActive: premiumRuleRow.IsActive,
            cityId: premiumRuleRowCityId);
    }

    public bool CanMapToEf(IRiskConfiguration riskConfiguration)
        => riskConfiguration is CityRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration riskConfiguration)
    {
        var cityRiskConfiguration = (CityRiskConfiguration)riskConfiguration;

        return new PremiumRule
        {
            PremiumRuleKey = cityRiskConfiguration.Id,
            RuleKind = Kind,
            Name = cityRiskConfiguration.Name,
            Percentage = cityRiskConfiguration.Percentage,
            IsActive = cityRiskConfiguration.IsActive,
            CityId = cityRiskConfiguration.CityId
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration riskConfiguration)
    {
        var cityRiskConfiguration = (CityRiskConfiguration)riskConfiguration;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = cityRiskConfiguration.Name;
        row.Percentage = cityRiskConfiguration.Percentage;
        row.IsActive = cityRiskConfiguration.IsActive;
        row.CityId = cityRiskConfiguration.CityId;
    }
}