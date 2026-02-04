using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class CityRiskConfiguration : RiskFactorConfiguration<CityRiskConfiguration>
{
    public int CityId { get; }
    
    private CityRiskConfiguration(Guid id,
              string name, decimal percentage, 
            bool isActive, int cityId) : base(id, name, percentage, isActive)
    {
        CityId = cityId;
        ValidateInvariants();
    }

    public static CityRiskConfiguration Create(
        string name,
        decimal percentage,
        bool isActive,
        int cityId)
    {
        return new CityRiskConfiguration(Guid.NewGuid(), name, percentage, isActive, cityId);
    }
    public static CityRiskConfiguration Rehydrate(Guid id,
                                        string name,
                                        decimal percentage,
                                        bool isActive,
                                        int cityId) =>
    new CityRiskConfiguration(id, name, percentage,
                         isActive, cityId);

    public override bool IsApplicable(PolicyDraftContext ctx)
    {
        return IsActive && ctx.CityId == CityId;
    }
    
    private void ValidateCity()
    {
        if (CityId <= 0)
            throw new DomainException("Invalid CityId.");
    }

    protected override void ValidateInvariants()
    {
        base.ValidateInvariants();
        ValidateCity();
    }
}
