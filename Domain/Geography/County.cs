using Domain.Common;
namespace Domain.Geography;

public sealed class County
{   
    public Guid Id { get; }
    public string Name { get; }
    public Country Country { get; }

    private readonly List<City> _cities = new();
    public IReadOnlyCollection<City> Cities => _cities;

    internal County(Guid id, string name, Country country)
    {
        Id = id;
        Name = name;
        Country = country;
    }

    public Result<City> AddCity(string name) {
        if (_cities.Any(c => c.Name == name))
            return Result<City>.Fail(ErrorType.Conflict, "Duplicate City");

        var city = new City(Guid.NewGuid(), name, this);
        _cities.Add(city);
        return Result<City>.Ok(city);
    }
}
