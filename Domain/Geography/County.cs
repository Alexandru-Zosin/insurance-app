using Domain.Common;
namespace Domain.Geography;

public sealed record County(int Id, int CountryId, string Name)
{
    public static County Create(int id, int countryId, string name)
    {
        if (id < 0)
            throw new DomainException(GeographyConstants.CountyIdMustBePositiveMsg);

        if (countryId < 0)
            throw new DomainException(GeographyConstants.CountryIdMustBePositiveNoSpaceMsg);

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(GeographyConstants.CountyNameRequiredMsg);

        return new County(id, countryId, name);
    }
}
