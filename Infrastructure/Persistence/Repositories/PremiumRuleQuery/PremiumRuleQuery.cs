using Application.Repositories;
using Domain.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories.PremiumRuleQuery;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Queries;

public sealed class PremiumRuleQuery : IPremiumRuleQuery
{
    private readonly InsuranceDbContext _db;
    private readonly IPremiumRuleMapperRegistry _registry;

    public PremiumRuleQuery(InsuranceDbContext db, IPremiumRuleMapperRegistry registry)
    {
        _db = db;
        _registry = registry;
    }

    public async Task<IReadOnlyList<IPremiumRule>> GetActiveAsync(CancellationToken ct = default)
    {
        var rows = await _db.PremiumRules
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var result = new List<IPremiumRule>(rows.Count);

        foreach (var row in rows)
        {
            var mapper = _registry.ResolveForRow(row);
            result.Add(mapper.ToDomain(row));
        }

        return result;
    }
}
