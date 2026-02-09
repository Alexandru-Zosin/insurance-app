using Application.Repositories;
using Domain.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories.PremiumRuleQuery;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Queries;

public sealed class PremiumRuleQuery : IPremiumRuleQuery
{
    private readonly InsuranceDbContext _dbContext;
    private readonly IPremiumRuleMapperRegistry _ruleMapperRegistry;

    public PremiumRuleQuery(InsuranceDbContext db, IPremiumRuleMapperRegistry registry)
    {
        _dbContext = db;
        _ruleMapperRegistry = registry;
    }

    public async Task<IReadOnlyList<IPremiumRule>> GetActiveAsync(CancellationToken ct = default)
    {
        var activeRuleRows = await _dbContext.PremiumRules
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var rules = new List<IPremiumRule>(activeRuleRows.Count);

        foreach (var ruleRow in activeRuleRows)
        {
            var ruleMapper = _ruleMapperRegistry.ResolveMapperForRow(ruleRow);
            rules.Add(ruleMapper.MapToDomain(ruleRow));
        }

        return rules;
    }
}
