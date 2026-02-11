using Domain.Currencies;

namespace Application.Services.Shared.DTOs.CurrencyDTOs;

public sealed record CurrencyDto(
    string Code,
    string Name,
    decimal ExchangeRateToBase,
    bool IsActive)
{
    public static CurrencyDto From(Currency c) => new(c.Code, c.Name, c.ExchangeRateToBase, c.IsActive);

    public Currency MapToDomain() => Currency.Create(Code, Name, ExchangeRateToBase, IsActive);
}
