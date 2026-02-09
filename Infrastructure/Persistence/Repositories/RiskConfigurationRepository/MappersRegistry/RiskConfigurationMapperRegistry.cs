using Domain.Configurations;
using Infrastructure.Persistence.Models;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.MappersRegistry;


public sealed class RiskConfigurationMapperRegistry : IRiskConfigurationMapperRegistry
{
    private readonly IReadOnlyList<IRiskConfigurationMapper> _riskConfigurationMappers;

    public RiskConfigurationMapperRegistry(IEnumerable<IRiskConfigurationMapper> riskConfigurationMappers)
    {
        if (riskConfigurationMappers is null)
            throw new ArgumentNullException(nameof(riskConfigurationMappers));

        _riskConfigurationMappers = riskConfigurationMappers.ToList();

        if (_riskConfigurationMappers.Count == 0)
            throw new InvalidOperationException("No risk configuration mappers registered.");
    }

    public IRiskConfigurationMapper ResolveMapperForRiskConfiguration(IRiskConfiguration riskConfigurationInstance)
    {
        foreach (var mapper in _riskConfigurationMappers)
        {
            if (mapper.CanMapToEf(riskConfigurationInstance))
                return mapper;
        }

        throw new NotSupportedException(
            $"Unsupported risk configuration type '{riskConfigurationInstance.GetType().Name}'.");
    }

    public bool TryResolveMapperForPremiumRuleRow(PremiumRule premiumRuleRow,
        out IRiskConfigurationMapper? resolvedRiskMapper)
    {
        foreach (var mapper in _riskConfigurationMappers)
        {
            if (mapper.CanMapToDomain(premiumRuleRow))
            {
                resolvedRiskMapper = mapper;
                return true;
            }
        }

        resolvedRiskMapper = null;
        return false;
    }
}