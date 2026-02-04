using Domain.Configurations;
using Domain.Policies;
using Domain.Shared;

namespace Domain.Services;

public sealed class PremiumCalculatorService : IPremiumCalculatorService
{
    public Money CalculateFinalPremium(PolicyDraftContext ctx, IEnumerable<IPremiumRule> rules)
    {
        var totalAddedPercentage = rules.Where(r => r.IsApplicable(ctx)).Sum(r => r.Percentage);
        return ctx.BasePremium with { Amount = ctx.BasePremium.Amount * (1m + totalAddedPercentage) };
    }
}
