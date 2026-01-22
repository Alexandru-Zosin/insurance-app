using Domain.Common;
namespace Domain.Shared;

public sealed record Address(string Street, string Number)
{
    public static Result<Address> Create(string street, string number) {
        if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(number))
            return Result<Address>.Fail(ErrorType.Validation, "Invalid address");

        return Result<Address>.Ok(new Address(street, number));
    }
}
