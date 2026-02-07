using Domain.Common;
namespace Domain.Geography;

public sealed record County(int Id, int CountryId, string Name)
{
    public static County Create(int id, int countryId, string name)
    {
        if (id <= 0)
            throw new DomainException("County Id must be positive.");

        if (countryId <= 0)
            throw new DomainException("CountryId must be positive.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("County name is required.");

        return new County(id, countryId, name);
    }
}
