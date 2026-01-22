using Domain.Common;
namespace Domain.Geography;

public class Country
{
    public Guid Id;
    public string Name;

    private readonly List<County> _counties = new();
    public IReadOnlyCollection<County> counties => _counties;

    public Country(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name required");

        Id = id;
        Name = name;
    }

    public Result<County> AddCounty(string name)
    {
        if (_counties.Any(c => c.Name == name))
            return Result<County>.Fail(ErrorType.Validation, "Duplicate country");

        var county = new County(Guid.NewGuid(), name, this);
        _counties.Add(county);

        return Result<County>.Ok(county);
    }
}
