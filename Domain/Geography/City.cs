namespace Domain.Geography;

public sealed record City
{
    public int Id { get; }
    public string Name { get; }

    public City(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name required");

        Id = id;
        Name = name;
    }
}
