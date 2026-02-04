using Domain.Common;
using Domain.Policies;
using Domain.Shared;

namespace Domain.Configurations;

public sealed class FeeConfiguration : IPremiumRule
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public FeeType Type { get; private set; }
    public decimal Percentage { get; private set; }
    public ValidityPeriod ValidityPeriod { get; private set; }
    public bool IsActive { get; private set; }

    private FeeConfiguration(
        Guid id,
        string name,
        FeeType type,
        decimal percentage,
        ValidityPeriod validityPeriod,
        bool isActive)
    {
        Id = id;
        Name = name;
        Type = type;
        Percentage = percentage;
        ValidityPeriod = validityPeriod;
        IsActive = isActive;

        ValidateInvariants();
    }

    public static FeeConfiguration Create(
        string name,
        FeeType type,
        decimal percentage,
        ValidityPeriod validityPeriod,
        bool isActive)
    {
        return new FeeConfiguration(Guid.NewGuid(), name, type, percentage, validityPeriod, isActive);
    }

    public static FeeConfiguration Rehydrate(
        Guid id,
        string name,
        FeeType type,
        decimal percentage,
        ValidityPeriod validityPeriod,
        bool isActive)
    {
        return new FeeConfiguration(id, name, type, percentage, validityPeriod, isActive);
    }

    public FeeConfiguration Activate()
    {
        ValidateInvariants();
        IsActive = true;
        return this;
    }

    public FeeConfiguration Deactivate()
    {
        IsActive = false;
        return this;
    }

    public FeeConfiguration UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        return this;
    }

    public FeeConfiguration UpdatePercentage(decimal percentage)
    {
        ValidatePercentage(percentage);
        Percentage = percentage;
        return this;
    }

    public FeeConfiguration UpdateValidity(ValidityPeriod validityPeriod)
    {
        ValidateValidity(validityPeriod);
        ValidityPeriod = validityPeriod;
        return this;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Invalid fee configuration name.");
    }

    private static void ValidatePercentage(decimal percentage)
    {
        if (percentage < 0.0m || percentage > 1.0m)
            throw new DomainException("Invalid fee configuration percentage.");
    }

    private static void ValidateValidity(ValidityPeriod validityPeriod)
    {
        if (validityPeriod is null)
            throw new DomainException("Invalid validity period for fee configuration.");
    }

    private void ValidateInvariants()
    {
        ValidateName(Name);
        ValidatePercentage(Percentage);
        ValidateValidity(ValidityPeriod);
    }
    public bool IsApplicable(PolicyDraftContext ctx) => IsActive &&
        ValidityPeriod.Contains(ctx.DraftDate);
}