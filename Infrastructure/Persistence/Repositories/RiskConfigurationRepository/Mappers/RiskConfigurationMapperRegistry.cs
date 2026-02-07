using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public sealed class RiskConfigurationMapperRegistry : IRiskConfigurationMapperRegistry
{
    private readonly IReadOnlyList<IRiskConfigurationMapper> _mappers;

    public RiskConfigurationMapperRegistry(IEnumerable<IRiskConfigurationMapper> mappers)
    {
        _mappers = mappers?.ToList() ?? throw new ArgumentNullException(nameof(mappers));
        if (_mappers.Count == 0)
            throw new InvalidOperationException("No risk configuration mappers registered.");
    }

    public bool TryResolveForRow(PremiumRule row, out IRiskConfigurationMapper mapper)
    {
        foreach (var m in _mappers)
        {
            if (m.CanMaterialize(row))
            {
                mapper = m;
                return true;
            }
        }

        mapper = null!;
        return false;
    }

    public IRiskConfigurationMapper ResolveForRow(PremiumRule row)
    {
        if (TryResolveForRow(row, out var mapper))
            return mapper;

        throw new NotSupportedException($"Unsupported RuleKind '{row.RuleKind}'.");
    }

    public IRiskConfigurationMapper ResolveForAggregate(IRiskConfiguration aggregate)
    {
        foreach (var m in _mappers)
        {
            if (m.CanPersist(aggregate))
                return m;
        }

        throw new NotSupportedException($"Unsupported risk aggregate type '{aggregate.GetType().Name}'.");
    }
}
