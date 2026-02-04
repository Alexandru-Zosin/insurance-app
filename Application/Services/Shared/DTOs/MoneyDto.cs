using Domain.Shared;

namespace Application.Services.Shared.DTOs;

public sealed record MoneyDto(decimal Amount, string CurrencyCode)
{
    public static MoneyDto? From(Money? money) =>
        money == null ? null : new(money.Amount, money.CurrencyCode);

    public Money ToDomain() => Money.Create(Amount, CurrencyCode);
}
