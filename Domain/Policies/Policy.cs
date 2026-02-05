using Domain.Common;
using Domain.Shared;

namespace Domain.Policies;
public sealed class Policy
{
    public Guid Number { get; }
    public Guid ClientId { get; }
    public Guid BuildingId { get; }
    public Guid BrokerId { get; }
    public PolicyStatus Status { get; private set; }
    public ValidityPeriod Tenure { get; private set; }
    public Money BasePremium { get; }
    public string CurrencyCode { get; private set; }
    public Money FinalPremium { get; private set; }
    public DateOnly CreationDate { get; private set; }
    public DateOnly? LastUpdateDate { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateOnly? CancellationEffectiveDate { get; private set; }

    private Policy(
        Guid number,
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        ValidityPeriod tenure,
        Money basePremium,
        string currencyCode,
        Money finalPremium,
        PolicyStatus status,
        DateOnly creationDate,
        DateOnly? lastUpdateDate,
        string? cancellationReason,
        DateOnly? cancellationEffectiveDate)
    {
        Number = number;
        ClientId = clientId;
        BuildingId = buildingId;
        BrokerId = brokerId;

        Tenure = tenure;
        BasePremium = basePremium;
        CurrencyCode = currencyCode;
        FinalPremium = finalPremium;

        Status = status;
        CreationDate = creationDate;
        LastUpdateDate = lastUpdateDate;

        CancellationReason = cancellationReason;
        CancellationEffectiveDate = cancellationEffectiveDate;

        ValidateInvariants();
    }

    public static Policy CreateDraft(
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        ValidityPeriod tenure,
        Money basePremium,
        string currencyCode,
        Money preliminaryFinalPremium,
        DateOnly creationDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        return new Policy(
            number: Guid.NewGuid(),
            clientId: clientId,
            buildingId: buildingId,
            brokerId: brokerId,
            tenure: tenure,
            basePremium: basePremium,
            currencyCode: currencyCode,
            finalPremium: preliminaryFinalPremium,
            status: PolicyStatus.Draft,
            creationDate: today,
            lastUpdateDate: null,
            cancellationReason: null,
            cancellationEffectiveDate: null);
    }

    public static Policy Rehydrate(
        Guid number,
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        ValidityPeriod tenure,
        Money basePremium,
        string currencyCode,
        Money finalPremium,
        PolicyStatus status,
        DateOnly creationDate,
        DateOnly? lastUpdateDate,
        string? cancellationReason,
        DateOnly? cancellationEffectiveDate)
    {
        return new Policy(
            number: number,
            clientId: clientId,
            buildingId: buildingId,
            brokerId: brokerId,
            tenure: tenure,
            basePremium: basePremium,
            currencyCode: currencyCode,
            finalPremium: finalPremium,
            status: status,
            creationDate: creationDate,
            lastUpdateDate: lastUpdateDate,
            cancellationReason: cancellationReason,
            cancellationEffectiveDate: cancellationEffectiveDate);
    }

    public Policy SetFinalPremium(Money finalPremium, DateOnly updateDate)
    {
        EnsureFinalPremiumSettable(finalPremium);

        FinalPremium = finalPremium;
        LastUpdateDate = DateOnly.FromDateTime(DateTime.Now);
        return this;
    }

    public Policy Activate()
    {
        EnsureActivatable();

        Status = PolicyStatus.Active;
        LastUpdateDate = DateOnly.FromDateTime(DateTime.Now);

        ValidateInvariants();
        return this;
    }

    public Policy Cancel(string reason, DateOnly cancellationEffectiveDate)
    {
        EnsureCancellable(reason);

        CancellationReason = reason;
        CancellationEffectiveDate = cancellationEffectiveDate;
        Status = PolicyStatus.Cancelled;
        LastUpdateDate = DateOnly.FromDateTime(DateTime.Now);

        return this;
    }

    private void EnsureFinalPremiumSettable(Money finalPremium)
    {
        if (Status != PolicyStatus.Draft)
            throw new DomainException("Final premium can only be set in Draft status.");

        if (finalPremium is null)
            throw new DomainException("Invalid final premium.");

        if (finalPremium.CurrencyCode != CurrencyCode)
            throw new DomainException("Final premium currency must match policy currency.");

        if (finalPremium.Amount < 0m)
            throw new DomainException("Final premium must be non-negative.");
    }


    private void EnsureActivatable()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        if (Status != PolicyStatus.Draft)
            throw new DomainException("Only Draft policies can be activated.");

        if (Tenure.StartDate < today)
            throw new DomainException("Policy start date cannot be in the past.");
    }

    private void EnsureCancellable(string reason)
    {
        if (Status != PolicyStatus.Active)
            throw new DomainException("Only Active policies can be cancelled.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required.");
    }
    
    private void ValidateReferences()
    {
        if (ClientId == Guid.Empty)
            throw new DomainException("Invalid Client Id.");

        if (BuildingId == Guid.Empty)
            throw new DomainException("Invalid Building Id.");

        if (BrokerId == Guid.Empty)
            throw new DomainException("Invalid Broker Id.");
    }
    private void ValidateCoreValues()
    {
        if (Tenure is null)
            throw new DomainException("Invalid Policy Period.");

        if (BasePremium is null)
            throw new DomainException("Invalid Base Premium.");

        if (FinalPremium is null)
            throw new DomainException("Invalid Final Premium.");

        if (CreationDate == default)
            throw new DomainException("Invalid Creation Date.");
    }

    private void ValidateCurrencies()
    {
        if (string.IsNullOrWhiteSpace(CurrencyCode))
            throw new DomainException("Invalid Currency.");

         if (BasePremium.CurrencyCode != CurrencyCode)
            throw new DomainException("Base premium currency must match policy currency.");

        if (FinalPremium.CurrencyCode != CurrencyCode)
            throw new DomainException("Final premium currency must match policy currency.");

        if (BasePremium.CurrencyCode != FinalPremium.CurrencyCode)
            throw new DomainException("Base premium currency must match final premium currency.");
    }

    private void ValidateInvariants()
    {
        ValidateReferences();
        ValidateCoreValues();
        ValidateCurrencies();
    }
}