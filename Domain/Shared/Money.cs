using Domain.Common;

namespace Domain.Shared;

public sealed record Money(decimal Amount, string CurrencyCode)
{
    public static Money Create(decimal amount, string currencyCode)
    {
        if (amount < 0)
            throw new DomainException("Amount must be greater than 0");

        if (string.IsNullOrEmpty(currencyCode))
            throw new DomainException("Currency code unspecified.");

        return new Money(amount, currencyCode);
    }
}
