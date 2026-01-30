using Domain.Common;

namespace Domain.Shared;

public sealed record Address
{
    public string Street { get; }
    public string Number { get; }

    private Address(string street, string number)
    {
        Street = street;
        Number = number;
    }

    public static Address Create(string street, string number) {
        if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(number))
            throw new DomainException("Street and number must be provided together.");
        
        return new Address(street, number);
    }

    public static Address? CreateOptional(string? street, string? number)
    {
        var hasStreet = !string.IsNullOrWhiteSpace(street);
        var hasNumber = !string.IsNullOrWhiteSpace(number);
        if (hasStreet && hasNumber)
            return null;

        return Create(street ?? string.Empty, number ?? string.Empty);
    }
}
