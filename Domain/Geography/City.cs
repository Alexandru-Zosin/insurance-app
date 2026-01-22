namespace Domain.Geography;

public sealed class City
{
    public Guid Id { get; }
    public string Name { get; }
    public County County { get; }

    internal City (Guid id, string name, County county)
    {
        id = Guid.NewGuid();
        Name = name;
        County = county;
    }
}
