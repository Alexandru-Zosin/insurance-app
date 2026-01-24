using Domain.Policies;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class PolicyRepository : IPolicyRepository
{
    private readonly InsuranceDbContext _db;

    public PolicyRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public async Task<Policy?> GetByIdAsync(
        Guid policyId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Policies
            .AsNoTracking()
            .SingleOrDefaultAsync(
                p => p.PolicyKey == policyId,
                cancellationToken)
            .ConfigureAwait(false);

        return ef == null ? null : Map(ef);
    }

    public async Task<IReadOnlyList<Policy>> GetByClientIdAsync(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        var entities = await _db.Policies
            .Where(p => p.Client.ClientKey == clientId)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entities
            .Select(Map)
            .ToList();
    }

    public async Task<IReadOnlyList<Policy>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var entities = await _db.Policies
            .Where(p => p.Building.BuildingKey == buildingId)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entities
            .Select(Map)
            .ToList();
    }

    public async Task AddAsync(
        Policy policy,
        CancellationToken cancellationToken)
    {
        var clientId = await _db.Clients
            .Where(c => c.ClientKey == policy.ClientId)
            .Select(c => c.ClientId)
            .SingleAsync(cancellationToken)
            .ConfigureAwait(false);

        var buildingId = await _db.Buildings
            .Where(b => b.BuildingKey == policy.BuildingId)
            .Select(b => b.BuildingId)
            .SingleAsync(cancellationToken)
            .ConfigureAwait(false);

        var brokerId = await _db.Brokers
            .Where(b => b.BrokerKey == policy.BrokerId)
            .Select(b => b.BrokerId)
            .SingleAsync(cancellationToken)
            .ConfigureAwait(false);

        await _db.Policies.AddAsync(
            new Models.Policy
            {
                PolicyKey = policy.Id,
                ClientId = clientId,
                BuildingId = buildingId,
                BrokerId = brokerId,
                PremiumAmount = policy.Premium.Amount,
                PremiumCurrency = policy.Premium.Currency,
                StartDate = policy.StartDate,
                EndDate = policy.EndDate
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    private static Policy Map(Models.Policy ef)
    {
        var money = Money.Create(ef.PremiumAmount, ef.PremiumCurrency).Value!;

        return Policy.Issue(
            ef.Client.ClientKey,
            ef.Building.BuildingKey,
            ef.Broker.BrokerKey,
            money,
            ef.StartDate,
            ef.EndDate).Value!;
    }
}
