namespace Domain.Policies;

internal static class PolicyConstants
{
    internal const decimal MoneyNonNegativeMin = 0.0m;

    internal const string FinalPremiumOnlyInDraftMsg = "Final premium can only be set in Draft status.";
    internal const string InvalidFinalPremiumMsg = "Invalid final premium.";
    internal const string FinalPremiumCurrencyMustMatchPolicyCurrencyMsg =
        "Final premium currency must match policy currency.";
    internal const string FinalPremiumMustBeNonNegativeMsg = "Final premium must be non-negative.";

    internal const string OnlyDraftPoliciesCanBeActivatedMsg = "Only Draft policies can be activated.";
    internal const string PolicyStartDateCannotBeInThePastMsg = "Policy start date cannot be in the past.";

    internal const string OnlyActivePoliciesCanBeCancelledMsg = "Only Active policies can be cancelled.";
    internal const string CancellationReasonRequiredMsg = "Cancellation reason is required.";

    internal const string InvalidClientIdMsg = "Invalid Client Id.";
    internal const string InvalidBuildingIdMsg = "Invalid Building Id.";
    internal const string InvalidBrokerIdMsg = "Invalid Broker Id.";

    internal const string InvalidPolicyPeriodMsg = "Invalid Policy Period.";
    internal const string InvalidBasePremiumMsg = "Invalid Base Premium.";
    internal const string InvalidFinalPremiumInvariantMsg = "Invalid Final Premium.";
    internal const string InvalidCreationDateMsg = "Invalid Creation Date.";

    internal const string InvalidCurrencyMsg = "Invalid Currency.";
    internal const string BasePremiumCurrencyMustMatchPolicyCurrencyMsg = "Base premium currency must match policy currency.";
    internal const string FinalPremiumCurrencyMustMatchPolicyCurrencyInvariantMsg =
        "Final premium currency must match policy currency.";
}
