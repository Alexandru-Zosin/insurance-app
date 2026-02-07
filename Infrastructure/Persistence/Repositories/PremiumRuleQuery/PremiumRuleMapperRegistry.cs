using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.PremiumRuleQuery;

public sealed class PremiumRuleMapperRegistry : IPremiumRuleMapperRegistry
{
    private readonly IReadOnlyList<IPremiumRuleMapper> _mappers;

    public PremiumRuleMapperRegistry(IEnumerable<IPremiumRuleMapper> mappers)
    {
        if (mappers is null) throw new ArgumentNullException(nameof(mappers));

        _mappers = mappers.ToList();
        if (_mappers.Count == 0)
            throw new InvalidOperationException("No premium rule mappers registered.");
    }

    public IPremiumRuleMapper ResolveForRow(PremiumRule row)
    {
        if (row is null) throw new ArgumentNullException(nameof(row));

        foreach (var m in _mappers)
            if (m.CanMaterialize(row))
                return m;

        throw new NotSupportedException($"Unsupported RuleKind '{row.RuleKind}'.");
    }
}
