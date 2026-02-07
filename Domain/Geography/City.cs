using Domain.Common;
namespace Domain.Geography;

public sealed record City(int Id, int CountyId, string Name)
{
    public static City Create(int id, int countyId, string name)
    {
        if (id < 0)
            throw new DomainException(GeographyConstants.CityIdMustBePositiveMsg);

        if (countyId < 0)
            throw new DomainException(GeographyConstants.CountyIdMustBePositiveMsg);

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(GeographyConstants.CityNameRequiredMsg);

        return new City(id, countyId, name);
    }
}
