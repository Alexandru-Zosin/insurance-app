using Domain.Common;
using Domain.Shared;
using Domain.Buildings;

namespace Domain.Policies.Services;

public sealed class PremiumCalculatorService
{
    public Result<Money> CalculatePremium(Building building)
    {
        var baseRate = 0.01m;
        var premiumAmount = building.InsuredValue.Amount
            * baseRate
            * building.RiskProfile.Coefficient;

        return Money.Create(premiumAmount, building.InsuredValue.Currency);
    }
}
