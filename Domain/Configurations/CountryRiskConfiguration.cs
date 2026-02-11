using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public sealed class CountryRiskConfiguration : IRiskConfiguration
{
    private readonly RiskConfigCore _core;

    public RiskConfigCore Core => _core;

    public Guid Id => _core.Id;
    public string Name => _core.Name;
    public decimal Percentage => _core.Percentage;
    public bool IsActive => _core.IsActive;

    public int CountryId { get; }

    private CountryRiskConfiguration(RiskConfigCore core, int countryId)
    {
        _core = core;
        CountryId = countryId;

        ValidateInvariants();
    }

    public static CountryRiskConfiguration Create(string name, decimal percentage, bool isActive, int countryId)
        => new(new RiskConfigCore(Guid.NewGuid(), name, percentage, isActive), countryId);

    public static CountryRiskConfiguration FromState(Guid id, string name, decimal percentage, bool isActive, int countryId)
        => new(new RiskConfigCore(id, name, percentage, isActive), countryId);

    private void ValidateInvariants()
    {
        if (CountryId <= 0)
            throw new DomainException(RiskConfigurationConstants.InvalidCountryIdMsg);
    }
    public bool IsApplicable(PolicyDraftContext ctx)
        => IsActive && ctx.CountryId == CountryId;
}
