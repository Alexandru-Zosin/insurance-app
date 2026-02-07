using Domain.Common;
namespace Domain.Configurations;

public sealed class RiskConfigCore
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal Percentage { get; private set; }
    public bool IsActive { get; private set; }

    public RiskConfigCore(Guid id, string name, decimal percentage, bool isActive)
    {
        Id = id;
        Name = name;
        Percentage = percentage;
        IsActive = isActive;
        ValidateInvariants();
    }

    public RiskConfigCore Activate()
    {
        ValidateInvariants();
        IsActive = true;
        return this;
    }

    public RiskConfigCore Deactivate()
    {
        IsActive = false;
        return this;
    }

    public RiskConfigCore UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        return this;
    }

    public RiskConfigCore UpdatePercentage(decimal percentage)
    {
        ValidatePercentage(percentage);
        Percentage = percentage;
        return this;
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

    private void ValidateInvariants()
    {
        ValidateName(Name);
        ValidatePercentage(Percentage);
    }
}
