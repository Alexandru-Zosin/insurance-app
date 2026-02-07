using Application.Repositories;
using Domain.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.FeeConfigurationRepository;

public sealed class FeeConfigurationRepository(InsuranceDbContext _db) : IFeeConfigurationRepository
{
    public async Task AddAsync(FeeConfiguration aggregate, CancellationToken ct = default)
    {
        var row = FeeConfigurationMapper.ToEfModel(aggregate);
        _db.Set<PremiumRule>().Add(row);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<FeeConfiguration?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Set<PremiumRule>()
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == id && r.RuleKind == "Fee", ct);

        return row is null ? null : FeeConfigurationMapper.ToDomain(row);
    }

    public async Task<IReadOnlyList<FeeConfiguration>> ListAsync(CancellationToken ct = default)
    {
        var rows = await _db.Set<PremiumRule>()
            .AsNoTracking()
            .Where(r => r.RuleKind == "Fee")
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

        return rows.Select(FeeConfigurationMapper.ToDomain).ToList();
    }

    public async Task UpdateAsync(FeeConfiguration aggregate, CancellationToken ct = default)
    {
        var row = await _db.Set<PremiumRule>()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == aggregate.Id && r.RuleKind == "Fee", ct);

        if (row is null)
            throw new InvalidOperationException($"FeeConfiguration '{aggregate.Id}' was not found.");

        FeeConfigurationMapper.UpdateEfModel(row, aggregate);

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Set<PremiumRule>()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == id && r.RuleKind == "Fee", ct);

        if (row is null)
            return;

        row.IsActive = false;
        await _db.SaveChangesAsync(ct);
    }
}
