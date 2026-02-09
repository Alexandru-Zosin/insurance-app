using Domain.Common;
namespace Domain.Geography;

public sealed record Country(int Id, string Name)
{
    public static Country Create(int id, string name)
    {
        if (id < 0)
            throw new DomainException(GeographyConstants.CountryIdMustBePositiveMsg);

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(GeographyConstants.CountryNameRequiredMsg);

        return new Country(id, name);
    }
}
