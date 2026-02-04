using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class CountryRiskConfiguration : RiskFactorConfiguration<CountryRiskConfiguration>
{
    public int CountryId { get; }

    private CountryRiskConfiguration(Guid id,
                                     string name,
                                     decimal percentage,
                                     bool isActive,
                                     int countryId)
        : base(id, name, percentage, isActive)
    {
        CountryId = countryId;
        ValidateInvariants();
    }

    public static CountryRiskConfiguration Create(string name,
                                                  decimal percentage,
                                                  bool isActive,
                                                  int countryId) =>
        new CountryRiskConfiguration(Guid.NewGuid(), name, percentage, isActive, countryId);

    public static CountryRiskConfiguration Rehydrate(Guid id,
                                                     string name,
                                                     decimal percentage,
                                                     bool isActive,
                                                     int countryId) =>
        new CountryRiskConfiguration(id, name, percentage, isActive, countryId);

    public override bool IsApplicable(PolicyDraftContext ctx) =>
        IsActive && ctx.CountryId == CountryId;

    protected override void ValidateInvariants()
    {
        base.ValidateInvariants();
        if (CountryId <= 0)
            throw new DomainException("Invalid CountryId.");
    }
}
