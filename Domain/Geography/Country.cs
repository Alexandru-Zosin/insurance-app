namespace Domain.Geography;

public sealed record Country
{
    public int Id { get; }
    public string Name { get; }

    public Country(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Country name required");

        Id = id;
        Name = name;
    }
}
