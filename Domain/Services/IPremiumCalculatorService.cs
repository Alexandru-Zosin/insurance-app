using Domain.Configurations;
using Domain.Policies;
using Domain.Shared;

namespace Domain.Services
{
    public interface IPremiumCalculatorService
    {
        Money CalculateFinalPremium(PolicyDraftContext ctx, IEnumerable<IPremiumRule> rules);
    }
}