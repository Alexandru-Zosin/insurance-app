namespace Domain.Shared;

public sealed record RiskProfile(bool FloodRisk, bool EarthquakeRisk)
{
    public decimal Coefficient => 1.0m
        + (FloodRisk ? 0.2m : 0m)
        + (EarthquakeRisk ? 0.3m : 0m);
}
