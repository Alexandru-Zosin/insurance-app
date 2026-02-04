namespace Domain.Shared;

public sealed record RiskTag(RiskCategory RiskCategory) {
    // extra fields possible to add in future: Description, Weight etc.
}