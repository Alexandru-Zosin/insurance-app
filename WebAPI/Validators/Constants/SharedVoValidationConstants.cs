using System.Text.RegularExpressions;
namespace WebAPI.Validators.Constants;

public static class SharedVoValidationConstants
{
    // AddressDto
    public const int StreetMaxLength = 200;
    public const int NumberMaxLength = 32;

    // ContactInfo
    public const int EmailMaxLength = 254;
    public const int PhoneMaxLength = 32;

    public static readonly Regex PhoneRegex = new(
        pattern: @"^\+?[0-9\s().-]{6,32}$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // IdentificationNumberDto
    public const int IdentificationNumberMaxLength = 64;

    // MoneyDto
    public const int CurrencyCodeLength = 3;

    // Patterns (format-only; avoid business rules)
    public static readonly Regex AddressNumberRegex = new(
        pattern: @"^[A-Za-z0-9][A-Za-z0-9\s\-\/]*$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static readonly Regex IdentificationNumberRegex = new(
        pattern: @"^[A-Za-z0-9][A-Za-z0-9\-\/\s]*$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static readonly Regex CurrencyCodeRegex = new(
        pattern: "^[A-Z]{3}$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
