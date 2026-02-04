using Domain.Common;
using Domain.Policies;

namespace Domain.Configurations;

public abstract class RiskFactorConfiguration<TDerivedSelf> : IPremiumRule
        where TDerivedSelf : RiskFactorConfiguration<TDerivedSelf>
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public decimal Percentage { get; private set; }
    public bool IsActive { get; private set; }

    protected RiskFactorConfiguration(
        Guid id,
        string name,
        decimal percentage,
        bool isActive)
    {
        Id = id;
        Name = name;
        Percentage = percentage;
        IsActive = isActive;
    }

    public TDerivedSelf Activate()
    {
        ValidateInvariants();
        IsActive = true;
        return (TDerivedSelf)this;
    }

    public TDerivedSelf Deactivate()
    {
        IsActive = false;
        return (TDerivedSelf)this;
    }

    public TDerivedSelf UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        return (TDerivedSelf)this;
    }

    public TDerivedSelf UpdatePercentage(decimal percentage)
    {
        ValidatePercentage(percentage);
        Percentage = percentage;
        return (TDerivedSelf)this;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Invalid risk factor configuration name.");
    }

    private static void ValidatePercentage(decimal percentage)
    {
        if (percentage < 0.0m || percentage > 1.0m)
            throw new DomainException("Invalid risk factor configuration percentage.");
    }

    protected virtual void ValidateInvariants()
    {
        ValidateName(Name);
        ValidatePercentage(Percentage);
    }
    public abstract bool IsApplicable(PolicyDraftContext ctx);
}
