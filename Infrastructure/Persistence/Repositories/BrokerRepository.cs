using Domain.Brokers;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository : IBrokerRepository
{
    private readonly InsuranceDbContext _db;

    public BrokerRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public async Task<Domain.Brokers.Broker?> GetByIdAsync(
        Guid brokerId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Brokers
            .SingleOrDefaultAsync(b => b.BrokerKey == brokerId, cancellationToken)
            .ConfigureAwait(false);

        if (ef == null)
            return null;

        var broker = new Domain.Brokers.Broker(ef.BrokerKey, ef.Name);

        if (!ef.IsActive)
            broker.Deactivate();

        return broker;
    }

    public async Task AddAsync(
        Domain.Brokers.Broker broker,
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
        Domain.Brokers.Broker broker,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Brokers
            .SingleAsync(b => b.BrokerKey == broker.Id, cancellationToken)
            .ConfigureAwait(false);

        ef.IsActive = broker.IsActive;
    }
}
