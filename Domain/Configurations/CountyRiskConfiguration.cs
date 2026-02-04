using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class CountyRiskConfiguration : RiskFactorConfiguration<CountyRiskConfiguration>
{
    public int CountyId { get; }

    private CountyRiskConfiguration(Guid id,
                                    string name,
                                    decimal percentage,
                                    bool isActive,
                                    int countyId)
        : base(id, name, percentage, isActive)
    {
        CountyId = countyId;
        ValidateInvariants();
    }

    public static CountyRiskConfiguration Create(string name,
                                                 decimal percentage,
                                                 bool isActive,
                                                 int countyId) =>
        new CountyRiskConfiguration(Guid.NewGuid(), name, percentage, isActive, countyId);

    public static CountyRiskConfiguration Rehydrate(Guid id,
                                                    string name,
                                                    decimal percentage,
                                                    bool isActive,
                                                    int countyId) =>
        new CountyRiskConfiguration(id, name, percentage, isActive, countyId);

    public override bool IsApplicable(PolicyDraftContext ctx) =>
        IsActive && ctx.CountyId == CountyId;

    protected override void ValidateInvariants()
    {
        base.ValidateInvariants();
        if (CountyId <= 0)
            throw new DomainException("Invalid CountyId.");
    }
}
