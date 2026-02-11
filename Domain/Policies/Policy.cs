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

    public static Policy FromState(
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
            throw new DomainException(PolicyConstants.FinalPremiumOnlyInDraftMsg);

        if (finalPremium is null)
            throw new DomainException(PolicyConstants.InvalidFinalPremiumMsg);

        if (finalPremium.CurrencyCode != CurrencyCode)
            throw new DomainException(PolicyConstants.FinalPremiumCurrencyMustMatchPolicyCurrencyMsg);

        if (finalPremium.Amount < PolicyConstants.MoneyNonNegativeMin)
            throw new DomainException(PolicyConstants.FinalPremiumMustBeNonNegativeMsg);
    }


    private void EnsureActivatable()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        if (Status != PolicyStatus.Draft)
            throw new DomainException(PolicyConstants.OnlyDraftPoliciesCanBeActivatedMsg);

        if (Tenure.StartDate < today)
            throw new DomainException(PolicyConstants.PolicyStartDateCannotBeInThePastMsg);
    }

    private void EnsureCancellable(string reason)
    {
        if (Status != PolicyStatus.Active)
            throw new DomainException(PolicyConstants.OnlyActivePoliciesCanBeCancelledMsg);

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException(PolicyConstants.CancellationReasonRequiredMsg);
    }

    private void ValidateReferences()
    {
        if (ClientId == Guid.Empty)
            throw new DomainException(PolicyConstants.InvalidClientIdMsg);

        if (BuildingId == Guid.Empty)
            throw new DomainException(PolicyConstants.InvalidBuildingIdMsg);

        if (BrokerId == Guid.Empty)
            throw new DomainException(PolicyConstants.InvalidBrokerIdMsg);
    }
    private void ValidateCoreValues()
    {
        if (Tenure is null)
            throw new DomainException(PolicyConstants.InvalidPolicyPeriodMsg);

        if (BasePremium is null)
            throw new DomainException(PolicyConstants.InvalidBasePremiumMsg);

        if (FinalPremium is null)
            throw new DomainException(PolicyConstants.InvalidFinalPremiumInvariantMsg);

        if (CreationDate == default)
            throw new DomainException(PolicyConstants.InvalidCreationDateMsg);
    }

    private void ValidateCurrencies()
    {
        if (string.IsNullOrWhiteSpace(CurrencyCode))
            throw new DomainException(PolicyConstants.InvalidCurrencyMsg);

        if (BasePremium.CurrencyCode != CurrencyCode)
            throw new DomainException(PolicyConstants.BasePremiumCurrencyMustMatchPolicyCurrencyMsg);

        if (FinalPremium.CurrencyCode != CurrencyCode)
            throw new DomainException(PolicyConstants.FinalPremiumCurrencyMustMatchPolicyCurrencyInvariantMsg);
    }

    private void ValidateInvariants()
    {
        ValidateReferences();
        ValidateCoreValues();
        ValidateCurrencies();
    }
}