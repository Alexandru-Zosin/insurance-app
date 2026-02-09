using System.Text.RegularExpressions;

namespace WebAPI.Validators.Constants;

public static class BrokerValidationConstants
{
    // BrokerCoreDto
    public const int BrokerCodeMaxLength = 32;
    public const int BrokerNameMaxLength = 200;

    public static readonly Regex BrokerCodeRegex = new(
        pattern: "^[A-Z0-9_-]+$",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
