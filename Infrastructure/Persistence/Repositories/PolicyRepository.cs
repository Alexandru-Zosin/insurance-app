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

    public Policy? GetById(Guid policyId)
    {
        var ef = _db.Policies
            .AsNoTracking()
            .SingleOrDefault(p => p.PolicyKey == policyId);

        return ef == null ? null : Map(ef);
    }

    public IReadOnlyList<Policy> GetByClientId(Guid clientId)
    {
        return _db.Policies
            .Where(p => p.Client.ClientKey == clientId)
            .AsNoTracking()
            .Select(Map)
            .ToList();
    }

    public IReadOnlyList<Policy> GetByBuildingId(Guid buildingId)
    {
        return _db.Policies
            .Where(p => p.Building.BuildingKey == buildingId)
            .AsNoTracking()
            .Select(Map)
            .ToList();
    }

    public void Add(Policy policy)
    {
        var clientId = _db.Clients
            .Where(c => c.ClientKey == policy.ClientId)
            .Select(c => c.ClientId)
            .Single();

        var buildingId = _db.Buildings
            .Where(b => b.BuildingKey == policy.BuildingId)
            .Select(b => b.BuildingId)
            .Single();

        var brokerId = _db.Brokers
            .Where(b => b.BrokerKey == policy.BrokerId)
            .Select(b => b.BrokerId)
            .Single();

        _db.Policies.Add(new Models.Policy
        {
            PolicyKey = policy.Id,
            ClientId = clientId,
            BuildingId = buildingId,
            BrokerId = brokerId,
            PremiumAmount = policy.Premium.Amount,
            PremiumCurrency = policy.Premium.Currency,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate
        });
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
