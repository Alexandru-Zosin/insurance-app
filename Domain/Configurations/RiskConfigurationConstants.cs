namespace Domain.Configurations;

internal static class RiskConfigurationConstants
{
    public const decimal PercentageMin = 0.0m;
    public const decimal PercentageMax = 1.0m;

    public const string InvalidNameMsg = "Invalid risk factor configuration name.";
    public const string InvalidPercentageMsg = "Invalid risk factor configuration percentage.";

    internal const string InvalidCountryIdMsg = "Invalid CountryId.";
    internal const string InvalidCountyIdMsg = "Invalid CountyId.";
    internal const string InvalidCityIdMsg = "Invalid CityId.";
}
