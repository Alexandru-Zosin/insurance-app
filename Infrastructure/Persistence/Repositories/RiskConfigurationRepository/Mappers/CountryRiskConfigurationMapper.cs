using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class CountryRiskConfigurationMapper : IRiskConfigurationMapper
{
    private const string Kind = "RiskCountry";

    public bool CanMaterialize(PremiumRule row)
        => string.Equals(row.RuleKind, Kind, StringComparison.Ordinal);

    public IRiskConfiguration ToDomain(PremiumRule row)
    {
        if (row.CountryId is null)
            throw new InvalidOperationException("RiskCountry requires CountryId.");

        return CountryRiskConfiguration.Rehydrate(
            id: row.PremiumRuleKey,
            name: row.Name,
            percentage: row.Percentage,
            isActive: row.IsActive,
            countryId: row.CountryId.Value);
    }

    public bool CanPersist(IRiskConfiguration aggregate)
        => aggregate is CountryRiskConfiguration;

    public PremiumRule ToEfModel(IRiskConfiguration aggregate)
    {
        var a = (CountryRiskConfiguration)aggregate;

        return new PremiumRule
        {
            PremiumRuleKey = a.Id,
            RuleKind = Kind,
            Name = a.Name,
            Percentage = a.Percentage,
            IsActive = a.IsActive,
            CountryId = a.CountryId
        };
    }

    public void UpdateEfModel(PremiumRule row, IRiskConfiguration aggregate)
    {
        var a = (CountryRiskConfiguration)aggregate;

        if (!string.Equals(row.RuleKind, Kind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{Kind}'.");

        row.Name = a.Name;
        row.Percentage = a.Percentage;
        row.IsActive = a.IsActive;
        row.CountryId = a.CountryId;
    }
}
