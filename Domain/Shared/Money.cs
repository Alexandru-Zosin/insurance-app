using Domain.Common;
namespace Domain.Shared;

public sealed record Money(decimal Amount, string Currency)
{
    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount <= 0)
            return Result<Money>.Fail(ErrorType.Validation, "Amount must be positive");

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Fail(ErrorType.Validation, "Currency required");

        return Result<Money>.Ok(new Money(amount, currency));
    }
}
