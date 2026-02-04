using Domain.Common;
namespace Domain.Geography;

public sealed record Country(int Id, string Name)
{
    public static Country Create(int id, string name)
    {
        if (id <= 0)
            throw new DomainException("Country Id must be positive.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Country name is required.");

        return new Country(id, name);
    }
}
