using Application.Repositories;
using Domain.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository;

public sealed class RiskConfigurationRepository(
    InsuranceDbContext _db,
    IRiskConfigurationMapperRegistry _registry) : IRiskConfigurationRepository
{
    public void Add(IRiskConfiguration aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var mapper = _registry.ResolveForAggregate(aggregate);
        var row = mapper.ToEfModel(aggregate);

        _db.PremiumRules.Add(row);
    }

    public async Task UpdateAsync(IRiskConfiguration aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var row = await _db.PremiumRules
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == aggregate.Core.Id, ct)
            .ConfigureAwait(false);

        if (row is null)
            throw new InvalidOperationException("Risk factor not found.");

        var mapper = _registry.ResolveForAggregate(aggregate);
        mapper.UpdateEfModel(row, aggregate);
    }

    public async Task<IRiskConfiguration?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.PremiumRules
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.PremiumRuleKey == id, ct)
            .ConfigureAwait(false);

        if (row is null)
            return null;

        if (!_registry.TryResolveForRow(row, out var mapper))
            return null; // not a risk row (e.g., fee) or unsupported kind

        return mapper.ToDomain(row);
    }

    public async Task<IReadOnlyList<IRiskConfiguration>> ListAsync(CancellationToken ct = default)
    {
        var rows = await _db.PremiumRules
            .AsNoTracking()
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var result = new List<IRiskConfiguration>();

        foreach (var row in rows)
        {
            if (!_registry.TryResolveForRow(row, out var mapper))
                continue; // skip non-risk rows

            result.Add(mapper.ToDomain(row));
        }

        return result;
    }
}
