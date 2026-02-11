using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class CityRiskConfiguration : IRiskConfiguration
{
    private readonly RiskConfigCore _core;

    public RiskConfigCore Core => _core;

    public Guid Id => _core.Id;
    public string Name => _core.Name;
    public decimal Percentage => _core.Percentage;
    public bool IsActive => _core.IsActive;

    public int CityId { get; }

    private CityRiskConfiguration(RiskConfigCore core, int cityId)
    {
        _core = core;
        CityId = cityId;

        ValidateInvariants();
    }

    public static CityRiskConfiguration Create(string name, decimal percentage, bool isActive, int cityId)
        => new(new RiskConfigCore(Guid.NewGuid(), name, percentage, isActive), cityId);

    public static CityRiskConfiguration FromState(Guid id, string name, decimal percentage, bool isActive, int cityId)
        => new(new RiskConfigCore(id, name, percentage, isActive), cityId);

    public bool IsApplicable(PolicyDraftContext ctx)
        => IsActive && ctx.CityId == CityId;

    private void ValidateInvariants()
    {
        if (CityId <= 0)
            throw new DomainException(RiskConfigurationConstants.InvalidCityIdMsg);
    }
}
