using Application.Repositories;
using Domain.Configurations;

namespace Application.Services.Policies;

public sealed class PremiumRulesProvider(
    IFeeConfigurationRepository feeConfigurationRepository,
    IRiskConfigurationRepository riskConfigurationRepository) : IPremiumRulesProvider
{
    public async Task<IReadOnlyList<IPremiumRule>> GetActiveRulesAsync(CancellationToken ct = default)
    {
        var feeConfigurations = await feeConfigurationRepository.ListAsync(ct);
        var riskConfigurations = await riskConfigurationRepository.ListAsync(ct);

        var activeFeeRules = feeConfigurations
            .Where(fee => fee.IsActive)
            .Cast<IPremiumRule>();

        var activeRiskRules = riskConfigurations
            .Where(risk => risk.Core.IsActive)
            .Cast<IPremiumRule>();

        return activeFeeRules.Concat(activeRiskRules).ToArray();
    }
}
