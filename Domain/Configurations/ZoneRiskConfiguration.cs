using Domain.Configurations;
using Domain.Policies;
using Domain.Shared;

public sealed class ZoneRiskConfiguration
        : RiskFactorConfiguration<ZoneRiskConfiguration>
{
    public RiskTag Tag { get; }

    private ZoneRiskConfiguration(Guid id, string name,
                                  decimal pct, bool active, RiskTag tag)
        : base(id, name, pct, active)
    {
        Tag = tag;
        ValidateInvariants();
    }

    public static ZoneRiskConfiguration Create(string name,
                                               decimal pct,
                                               bool active,
                                               RiskTag tag) =>
        new(Guid.NewGuid(), name, pct, active, tag);

    public static ZoneRiskConfiguration Rehydrate(Guid id, string name,
                                                  decimal pct,
                                                  bool active,
                                                  RiskTag tag) =>
        new(id, name, pct, active, tag);

    public override bool IsApplicable(PolicyDraftContext ctx) =>
        IsActive && ctx.BuildingRiskTags.Contains(Tag);

    protected override void ValidateInvariants()
    {
        base.ValidateInvariants();
    }
}
