using Domain.Shared;

namespace Application.Services.Shared.DTOs;

public sealed record MoneyDto(decimal Amount, string CurrencyCode)
{
    public static MoneyDto From(Money money) => new(money.Amount, money.CurrencyCode);

    public Money MapToDomain() => Money.Create(Amount, CurrencyCode);
}
