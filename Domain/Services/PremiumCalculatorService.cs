using Domain.Configurations;
using Domain.Policies;
using Domain.Shared;

namespace Domain.Services;

public sealed class PremiumCalculatorService(
    Money BasePremium,
    PolicyDraftContext ctx, 
    IEnumerable<IPremiumRule> rules)
{
    public Money CalculateFinalPremium()
    {
        var totalAddedPercentage = rules.Where(r => r.IsApplicable(ctx)).Sum(r => r.Percentage);
        return BasePremium with { Amount = BasePremium.Amount * (1m + totalAddedPercentage)};
    }
}
