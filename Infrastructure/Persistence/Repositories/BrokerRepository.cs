using Application.Repositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Brokers;

namespace Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository : IBrokerRepository
{
    private readonly InsuranceDbContext _db;

    public BrokerRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public async Task<Broker?> GetByIdAsync(
        Guid brokerId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Brokers
            .SingleOrDefaultAsync(b => b.BrokerKey == brokerId, cancellationToken)
            .ConfigureAwait(false);

        if (ef == null)
            return null;

        var broker = new Broker(ef.BrokerKey, ef.Name);

        return broker;
    }

    public async Task AddAsync(
        Broker broker,
        CancellationToken cancellationToken)
    {
        await _db.Brokers.AddAsync(
            new Models.Broker
            {
                BrokerKey = broker.Id,
                Name = broker.Name,
                IsActive = broker.IsActive
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task UpdateAsync(
        Broker broker,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Brokers
            .SingleAsync(b => b.BrokerKey == broker.Id, cancellationToken)
            .ConfigureAwait(false);

        ef.IsActive = broker.IsActive;
    }
}
