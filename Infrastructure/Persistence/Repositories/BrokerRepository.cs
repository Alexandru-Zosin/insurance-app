using Domain.Brokers;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories;

public sealed class BrokerRepository : IBrokerRepository
{
    private readonly InsuranceDbContext _db;

    public BrokerRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public Domain.Brokers.Broker? GetById(Guid brokerId)
    {
        var ef = _db.Brokers.SingleOrDefault(b => b.BrokerKey == brokerId);
        var broker = new Domain.Brokers.Broker(ef.BrokerKey, ef.Name);
        if (!ef.IsActive)
            broker.Deactivate();

        return broker;
    }

    public void Add(Domain.Brokers.Broker broker)
    {
        _db.Brokers.Add(new Models.Broker
        {
            BrokerKey = broker.Id,
            Name = broker.Name,
            IsActive = broker.IsActive
        });
    }

    public void Update(Domain.Brokers.Broker broker)
    {
        var ef = _db.Brokers.Single(b => b.BrokerKey == broker.Id);
        ef.IsActive = broker.IsActive;
    }
}
