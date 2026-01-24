namespace Domain.Brokers;

public interface IBrokerRepository
{
    Broker? GetById(Guid brokerId);
    void Add(Broker broker);
    void Update(Broker broker);
}
