namespace Domain.Brokers;
internal static class BrokerConstants
{
    internal const decimal CommissionMin = 0.0m;
    internal const decimal CommissionMax = 1.0m;

    internal const string InvalidCodeMsg = "Invalid Broker Code.";
    internal const string InvalidNameMsg = "Invalid Broker Name.";
    internal const string InvalidContactInfoMissingMsg = "Invalid Contact Info (missing).";
    internal const string InvalidCommissionPercentageMsg = "Invalid Commission percentage.";
}
