using Domain.Configurations;
using Domain.Shared;
using global::Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.FeeConfigurationRepository;

internal static class FeeConfigurationMapper
{
    private const string FeeRuleKind = "Fee";

    public static PremiumRule ToEfModel(FeeConfiguration aggregate)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        return new PremiumRule
        {
            PremiumRuleKey = aggregate.Id,
            RuleKind = FeeRuleKind,
            Name = aggregate.Name,
            Percentage = aggregate.Percentage,
            IsActive = aggregate.IsActive,
            FeeType = aggregate.Type.ToString(),
            EffectiveFrom = aggregate.ValidityPeriod?.StartDate,
            EffectiveTo = aggregate.ValidityPeriod?.EndDate,
        };
    }

    public static void UpdateEfModel(PremiumRule row, FeeConfiguration aggregate)
    {
        if (row is null) throw new ArgumentNullException(nameof(row));
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        if (!string.Equals(row.RuleKind, FeeRuleKind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{FeeRuleKind}'.");

        if (row.PremiumRuleKey != aggregate.Id)
            throw new InvalidOperationException("Row key does not match aggregate id.");

        row.Name = aggregate.Name;
        row.Percentage = aggregate.Percentage;
        row.IsActive = aggregate.IsActive;
        row.FeeType = aggregate.Type.ToString();
        row.EffectiveFrom = aggregate.ValidityPeriod?.StartDate;
        row.EffectiveTo = aggregate.ValidityPeriod?.EndDate;
    }

    public static FeeConfiguration ToDomain(PremiumRule row)
    {
        if (row is null) throw new ArgumentNullException(nameof(row));
        if (!string.Equals(row.RuleKind, FeeRuleKind, StringComparison.Ordinal))
            throw new InvalidOperationException($"Expected RuleKind '{FeeRuleKind}'.");

        if (string.IsNullOrWhiteSpace(row.FeeType))
            throw new InvalidOperationException("Fee rule row requires FeeType.");

        var type = Enum.Parse<FeeType>(row.FeeType, ignoreCase: true);

        var validity = ValidityPeriod.CreateOptional(row.EffectiveFrom, row.EffectiveTo);

        return FeeConfiguration.Rehydrate(
            id: row.PremiumRuleKey,
            name: row.Name,
            type: type,
            percentage: row.Percentage,
            validityPeriod: validity!,
            isActive: row.IsActive);
    }
}
