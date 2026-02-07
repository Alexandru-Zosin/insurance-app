using Domain.Buildings;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class BuildingTypeRiskConfiguration : IRiskConfiguration
{
    private readonly RiskConfigCore _core;

    public RiskConfigCore Core => _core;

    public Guid Id => _core.Id;
    public string Name => _core.Name;
    public decimal Percentage => _core.Percentage;
    public bool IsActive => _core.IsActive;

    public BuildingType BuildingType { get; }

    private BuildingTypeRiskConfiguration(RiskConfigCore core, BuildingType buildingType)
    {
        _core = core ?? throw new ArgumentNullException(nameof(core));
        BuildingType = buildingType;
    }

    public static BuildingTypeRiskConfiguration Create(string name, decimal percentage, bool isActive, BuildingType buildingType)
        => new(new RiskConfigCore(Guid.NewGuid(), name, percentage, isActive), buildingType);

    public static BuildingTypeRiskConfiguration Rehydrate(Guid id, string name, decimal percentage, bool isActive, BuildingType buildingType)
        => new(new RiskConfigCore(id, name, percentage, isActive), buildingType);

    public bool IsApplicable(PolicyDraftContext ctx)
        => IsActive && ctx.BuildingType == BuildingType;
}
