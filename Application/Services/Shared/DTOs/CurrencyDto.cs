using Domain.Currencies;

namespace Application.Services.Shared.DTOs;

public sealed record CurrencyDto(
    string Code,
    string Name,
    decimal ExchangeRateToBase,
    bool IsActive)
{
    public static CurrencyDto? From(Currency? c) =>
        c is null ? null : new(c.Code, c.Name, c.ExchangeRateToBase, c.IsActive);

    public Currency ToDomain() => Currency.Create(Code, Name, ExchangeRateToBase, IsActive);
}
