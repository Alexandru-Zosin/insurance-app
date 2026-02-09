using Application.Repositories;
using Domain.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories.PremiumRuleQuery;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Queries;

public sealed class PremiumRuleQuery
    (InsuranceDbContext _dbContext, IPremiumRuleMapperRegistry _ruleMapperRegistry) : IPremiumRuleQuery
{
    public async Task<IReadOnlyList<IPremiumRule>> GetActiveAsync(CancellationToken ct = default)
    {
        var activeRuleRows = await _dbContext.PremiumRules
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(ct);

        var activeConcreteRules = new List<IPremiumRule>(activeRuleRows.Count);
        foreach (var ruleRow in activeRuleRows)
        {
            var ruleMapperForCurrentRow = _ruleMapperRegistry.ResolveMapperForRow(ruleRow);
            activeConcreteRules.Add(ruleMapperForCurrentRow.MapToDomain(ruleRow));
            // returns a concrete IPremiumRule implementation
        }

        return activeConcreteRules;
    }
}
