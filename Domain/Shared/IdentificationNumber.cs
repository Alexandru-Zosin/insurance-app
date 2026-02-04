using Domain.Common;
namespace Domain.Shared;

public sealed record IdentificationNumber(string Value)
{
    public static IdentificationNumber Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new DomainException("Identification not provided.");

        return new IdentificationNumber(value);
    }
}
