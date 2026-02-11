using Domain.Policies;

namespace Domain.Configurations;

public sealed class ZoneRiskConfiguration : IRiskConfiguration
{
    private readonly RiskConfigCore _core;

    public RiskConfigCore Core => _core;

    public Guid Id => _core.Id;
    public string Name => _core.Name;
    public decimal Percentage => _core.Percentage;
    public bool IsActive => _core.IsActive;

    public ZoneRiskCategory Category { get; }

    private ZoneRiskConfiguration(RiskConfigCore core, ZoneRiskCategory category)
    {
        _core = core;
        Category = category;
    }

    public static ZoneRiskConfiguration Create(string name, decimal pct, bool active, ZoneRiskCategory category)
        => new(new RiskConfigCore(Guid.NewGuid(), name, pct, active), category);

    public static ZoneRiskConfiguration FromState(Guid id, string name, decimal pct, bool active, ZoneRiskCategory category)
        => new(new RiskConfigCore(id, name, pct, active), category);

    public bool IsApplicable(PolicyDraftContext ctx)
        => IsActive && ctx.BuildingZoneRiskCategories.Contains(Category);
}
