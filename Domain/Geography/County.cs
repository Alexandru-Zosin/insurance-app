namespace Domain.Geography;

public sealed record County
{
    public int Id { get; }
    public string Name { get; }

    public County(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("County name required");

        Id = id;
        Name = name;
    }
}
