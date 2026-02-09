using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.PremiumRuleQuery;

public sealed class PremiumRuleMapperRegistry : IPremiumRuleMapperRegistry
{
    private readonly IReadOnlyList<IPremiumRuleMapper> _ruleMappers;

    public PremiumRuleMapperRegistry(IEnumerable<IPremiumRuleMapper> ruleMappers)
    {
        if (ruleMappers is null) throw new ArgumentNullException(nameof(ruleMappers));

        _ruleMappers = ruleMappers.ToList();
        if (_ruleMappers.Count == 0)
            throw new InvalidOperationException("No premium rule ruleMappers registered.");
    }

    public IPremiumRuleMapper ResolveMapperForRow(PremiumRule ruleRow)
    {
        if (ruleRow is null) throw new ArgumentNullException(nameof(ruleRow));

        foreach (var ruleMapper in _ruleMappers)
            if (ruleMapper.CanMap(ruleRow))
                return ruleMapper;

        throw new NotSupportedException($"Unsupported RuleKind '{ruleRow.RuleKind}'.");
    }
}
