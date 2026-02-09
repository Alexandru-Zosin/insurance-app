using Domain.Configurations;
using Infrastructure.Persistence.Models;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.MappersRegistry;

public interface IRiskConfigurationMapperRegistry
{
    /// <summary>
    /// Resolves the mapper that supports the runtime type of the provided risk configuration instance.
    /// </summary>
    /// <remarks>
    /// Use the returned mapper to persist the domain instance via MapToEf/MapOntoEf.
    /// </remarks>
    IRiskConfigurationMapper ResolveMapperForRiskConfiguration(IRiskConfiguration riskConfiguration);

    /// <summary>
    /// Attempts to resolve the specific mapper that can materialize a risk configuration from the given PremiumRule row.
    /// </summary>
    bool TryResolveMapperForPremiumRuleRow(PremiumRule ruleRow, out IRiskConfigurationMapper resolvedRiskMapper);
}