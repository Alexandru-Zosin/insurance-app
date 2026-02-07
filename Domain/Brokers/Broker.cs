using Domain.Common;
using Domain.Shared;

namespace Domain.Brokers;

public class Broker
{
    public Guid Id { get; init; }
    public string Code { get; }
    public string Name { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? CommissionPercentage { get; private set; }

    private Broker(Guid id, string code, string name, ContactInfo contactInfo,
        bool isActive, decimal? commissionPercentage)
    {
        Id = id;
        Code = code;
        Name = name;
        ContactInfo = contactInfo;
        IsActive = isActive;
        CommissionPercentage = commissionPercentage;

        ValidateInvariants();
    }

    public static Broker Create(string code, string name, ContactInfo contactInfo,
        bool isActive, decimal? commissionPercentage)
    {
        return new Broker(Guid.NewGuid(), code, name, contactInfo, isActive, commissionPercentage);
    }

     public static Broker Rehydrate(
        Guid id,
        string code,
        string name,
        ContactInfo contactInfo,
        bool isActive,
        decimal? commissionPercentage)
    {
        return new Broker(
            id,
            code,
            name,
            contactInfo,
            isActive,
            commissionPercentage);
    }

    public bool IsActiveForPolicyWork() => IsActive;

    public Broker Activate()
    {
        IsActive = true;
        return this;
    }

    public Broker Deactivate()
    {
        IsActive = false;
        return this;
    }

    public Broker UpdateContactInfo(ContactInfo contactInfo)
    {
        ValidateContactInfo(contactInfo);
        ContactInfo = contactInfo;
        return this;
    }

    public Broker UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        return this;
    }

    public Broker UpdateCommissionPercentage(decimal? commissionPercentage)
    {
        ValidateCommissionPercentage(commissionPercentage);
        CommissionPercentage = commissionPercentage;
        return this;
    }

    public static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(BrokerConstants.InvalidCodeMsg);
    }

    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(BrokerConstants.InvalidNameMsg);
    }

    public static void ValidateContactInfo(ContactInfo contactInfo)
    {
        if (contactInfo == null)
            throw new DomainException(BrokerConstants.InvalidContactInfoMissingMsg);
    }

    public static void ValidateCommissionPercentage(decimal? commissionPercentage)
    {
        if (commissionPercentage != null)
        {
            if (commissionPercentage < BrokerConstants.CommissionMin 
                || commissionPercentage > BrokerConstants.CommissionMax)
                throw new DomainException(BrokerConstants.InvalidCommissionPercentageMsg);
        }
    }

    public void ValidateInvariants()
    {
        ValidateCode(Code);
        ValidateName(Name);
        ValidateContactInfo(ContactInfo);
        ValidateCommissionPercentage(CommissionPercentage);
    }
}
