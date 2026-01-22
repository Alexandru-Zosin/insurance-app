using Domain.Common;
namespace Domain.Shared;

public sealed record IdentificationNumber
{
    public string Value { get; }

    private IdentificationNumber(string value)
    {
        Value = value;
    }

    public static Result<IdentificationNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<IdentificationNumber>.Fail(ErrorType.Validation, "Invalid identification number");

        return Result<IdentificationNumber>.Ok(new IdentificationNumber(value));
    }
}
