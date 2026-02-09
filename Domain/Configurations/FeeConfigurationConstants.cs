namespace Domain.Configurations;

internal static class FeeConfigurationConstants
{
    internal const decimal PercentageMin = 0.0m;
    internal const decimal PercentageMax = 1.0m;

    internal const string InvalidNameMsg = "Invalid fee configuration name.";
    internal const string InvalidPercentageMsg = "Invalid fee configuration percentage.";
    internal const string InvalidValidityPeriodMsg = "Invalid validity period for fee configuration.";
}
