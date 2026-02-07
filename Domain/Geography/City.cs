using Domain.Common;
namespace Domain.Geography;

public sealed record City(int Id, int CountyId, string Name)
{
    public static City Create(int id, int countyId, string name)
    {
        if (id <= 0)
            throw new DomainException("City Id must be positive.");

        if (countyId <= 0)
            throw new DomainException("CountyId must be positive.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("City name is required.");

        return new City(id, countyId, name);
    }
}
