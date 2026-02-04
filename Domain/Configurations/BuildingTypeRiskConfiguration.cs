using Domain.Buildings;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class BuildingTypeRiskConfiguration 
    : RiskFactorConfiguration<BuildingTypeRiskConfiguration>
{
    public BuildingType BuildingType { get; }

    private BuildingTypeRiskConfiguration(Guid id,
                                          string name,
                                          decimal percentage,
                                          bool isActive,
                                          BuildingType buildingType)
        : base(id, name, percentage, isActive)
    {
        BuildingType = buildingType;
        ValidateInvariants();
    }

    public static BuildingTypeRiskConfiguration Create(string name,
                                                       decimal percentage,
                                                       bool isActive,
                                                       BuildingType buildingType) =>
        new BuildingTypeRiskConfiguration(Guid.NewGuid(), name, percentage, isActive, buildingType);

    public static BuildingTypeRiskConfiguration Rehydrate(Guid id,
                                                          string name,
                                                          decimal percentage,
                                                          bool isActive,
                                                          BuildingType buildingType) =>
        new BuildingTypeRiskConfiguration(id, name, percentage, isActive, buildingType);

    public override bool IsApplicable(PolicyDraftContext ctx) =>
        IsActive && ctx.BuildingType == BuildingType;

    protected override void ValidateInvariants()
    {
        base.ValidateInvariants();
    }
}
