using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class CountryRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskCountry";

    public bool CanMapToDomain(PremiumRule premiumRuleRow)
        => string.Equals(premiumRuleRow.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration MapToDomain(PremiumRule premiumRuleRow)
    {
        if (premiumRuleRow.CountryId is null)
            throw new InvalidOperationException("RiskCountry requires CountryId.");

        var premiumRuleRowCountryId = premiumRuleRow.CountryId.Value;

        return CountryRiskConfiguration.FromState(
            id: premiumRuleRow.PremiumRuleKey,
            name: premiumRuleRow.Name,
            percentage: premiumRuleRow.Percentage,
            isActive: premiumRuleRow.IsActive,
            countryId: premiumRuleRowCountryId);
    }

    public bool CanMapToEf(IRiskConfiguration riskConfiguration)
        => riskConfiguration is CountryRiskConfiguration;

    public PremiumRule MapToEf(IRiskConfiguration riskConfiguration)
    {
        var countryRiskConfiguration = (CountryRiskConfiguration)riskConfiguration;

        return new PremiumRule
        {
            PremiumRuleKey = countryRiskConfiguration.Id,
            RuleKind = Kind,
            Name = countryRiskConfiguration.Name,
            Percentage = countryRiskConfiguration.Percentage,
            IsActive = countryRiskConfiguration.IsActive,
            CountryId = countryRiskConfiguration.CountryId
        };
    }

    public void MapOntoEf(PremiumRule row, IRiskConfiguration riskConfiguration)
    {
        var countryRiskConfiguration = (CountryRiskConfiguration)riskConfiguration;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = countryRiskConfiguration.Name;
        row.Percentage = countryRiskConfiguration.Percentage;
        row.IsActive = countryRiskConfiguration.IsActive;
        row.CountryId = countryRiskConfiguration.CountryId;
    }
}
