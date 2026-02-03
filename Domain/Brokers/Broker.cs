namespace Domain.Brokers;

public class Broker
{
    public Guid Id { get; }
    public string Name { get; }
    public bool IsActive { get; private set; }

    public Broker(Guid id, string name)
    {
        Id = id;
        Name = name;
        IsActive = true;
    }
}
