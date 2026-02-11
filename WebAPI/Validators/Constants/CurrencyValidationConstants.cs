using System.Text.RegularExpressions;

namespace WebAPI.Validators.Constants;

public static class CurrencyValidationConstants
{
    public const int CodeLength = 3;
    public const int NameMaxLength = 100;

    public const decimal ExchangeRateMinExclusive = 0m;
    public const decimal ExchangeRateMaxInclusive = 1_000_000m;

    public static readonly Regex CurrencyCodeRegex = new(
        pattern: "^[A-Z]{3}$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
