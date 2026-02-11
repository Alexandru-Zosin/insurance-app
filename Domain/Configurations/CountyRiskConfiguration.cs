using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class CountyRiskConfiguration : IRiskConfiguration
{
    private readonly RiskConfigCore _core;

    public RiskConfigCore Core => _core;

    public Guid Id => _core.Id;
    public string Name => _core.Name;
    public decimal Percentage => _core.Percentage;
    public bool IsActive => _core.IsActive;

    public int CountyId { get; }

    private CountyRiskConfiguration(RiskConfigCore core, int countyId)
    {
        _core = core;
        CountyId = countyId;

        ValidateInvariants();
    }

    public static CountyRiskConfiguration Create(string name, decimal percentage, bool isActive, int countyId)
        => new(new RiskConfigCore(Guid.NewGuid(), name, percentage, isActive), countyId);

    public static CountyRiskConfiguration FromState(Guid id, string name, decimal percentage, bool isActive, int countyId)
        => new(new RiskConfigCore(id, name, percentage, isActive), countyId);

    public bool IsApplicable(PolicyDraftContext ctx)
        => IsActive && ctx.CountyId == CountyId;

    private void ValidateInvariants()
    {
        if (CountyId <= 0)
            throw new DomainException(RiskConfigurationConstants.InvalidCountyIdMsg);
    }
}
