using Domain.Common;
namespace Domain.Currencies;

public sealed class Currency
{
    public string Code { get; }
    public string Name { get; }
    public decimal ExchangeRateToBase { get; private set; }
    public bool IsActive { get; private set; }

    private Currency(string code, string name, decimal exchangeRateToBase, bool isActive)
    {
        Code = code;
        Name = name;
        ExchangeRateToBase = exchangeRateToBase;
        IsActive = isActive;

        ValidateAggregate();
    }

    public static Currency Create(string code, string name, decimal exchangeRateToBase, bool isActive)
    {
        return new Currency(code, name, exchangeRateToBase, isActive);
    }

    public Currency UpdateExchangeRateToBase(decimal exchangeRateToBase)
    {
        ValidateExchangeRateToBase(exchangeRateToBase);
        ExchangeRateToBase = exchangeRateToBase;
        return this;
    }

    public Currency Activate()
    {
        IsActive = true;
        return this;
    }

    public Currency Deactivate()
    {
        IsActive = false;
        return this;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(CurrencyConstants.InvalidCodeMsg);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(CurrencyConstants.InvalidNameMsg);
    }

    private static void ValidateExchangeRateToBase(decimal exchangeRateToBase)
    {
        if (exchangeRateToBase <= CurrencyConstants.ExchangeRateMinExclusive)
            throw new DomainException(CurrencyConstants.InvalidExchangeRateMsg);
    }

    public void ValidateAggregate()
    {
        ValidateCode(Code);
        ValidateName(Name);
        ValidateExchangeRateToBase(ExchangeRateToBase);
    }
}
