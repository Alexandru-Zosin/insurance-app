using Application.Repositories;
using Domain.Currencies;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCurrency = Infrastructure.Persistence.Models.Currency;

namespace Infrastructure.Persistence.Repositories;

public sealed class CurrencyRepository(InsuranceDbContext _db) : ICurrencyRepository
{
    public void Add(Currency aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var ef = ToEfModel(aggregate);

        _db.Currencies.Add(ef);
    }

    public async Task UpdateAsync(Currency aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var code = NormalizeCode(aggregate.Code);
        var ef = await _db.Currencies
            .SingleOrDefaultAsync(x => x.Code == code, ct)
            .ConfigureAwait(false);

        if (ef is null)
            throw new InvalidOperationException("Currency not found.");

        UpdateEfModel(ef, aggregate);
    }

    public async Task<Currency?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));

        var normalized = NormalizeCode(code);

        var ef = await _db.Currencies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Code == normalized, ct)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<Currency>> ListAsync(CancellationToken ct = default)
    {
        var rows = await _db.Currencies
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static EfCurrency ToEfModel(Currency domain)
    {
        return new EfCurrency
        {
            Code = NormalizeCode(domain.Code),
            Name = domain.Name,
            ExchangeRateToBase = domain.ExchangeRateToBase,
            IsActive = domain.IsActive
        };
    }

    private static void UpdateEfModel(EfCurrency ef, Currency domain)
    {
        ef.Name = domain.Name;
        ef.ExchangeRateToBase = domain.ExchangeRateToBase;
        ef.IsActive = domain.IsActive;
    }

    private static Currency ToDomain(EfCurrency ef)
    {
        return Currency.Create(
            code: ef.Code,
            name: ef.Name,
            exchangeRateToBase: ef.ExchangeRateToBase,
            isActive: ef.IsActive);
    }
}
