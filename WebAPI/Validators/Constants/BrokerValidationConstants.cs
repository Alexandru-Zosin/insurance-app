using System.Text.RegularExpressions;

namespace WebAPI.Validators.Constants;

public static class BrokerValidationConstants
{
    // BrokerCoreDto
    public const int BrokerCodeMaxLength = 32;
    public const int BrokerNameMaxLength = 200;

    // ContactInfoDto
    public const int EmailMaxLength = 254; // common practical max for email addresses
    public const int PhoneMaxLength = 32;

    // Patterns (frontend-format validation only; no business rules)
    public static readonly Regex BrokerCodeRegex = new(
        pattern: "^[A-Z0-9_-]+$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // Basic email shape check; prefer EmailAddress() rule for most cases
    public static readonly Regex PhoneRegex = new(
        pattern: @"^\+?[0-9\s().-]{6,32}$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);

}
