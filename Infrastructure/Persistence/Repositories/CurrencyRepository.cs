using Application.Repositories;
using Domain.Currencies;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCurrency = Infrastructure.Persistence.Models.Currency;

namespace Infrastructure.Persistence.Repositories;

public sealed class CurrencyRepository(InsuranceDbContext _dbContext) : ICurrencyRepository
{
    public void Add(Currency currencyToAdd, CancellationToken ct = default)
    {
        if (currencyToAdd is null) throw new ArgumentNullException(nameof(currencyToAdd));

        var currencyRow = MapToEf(currencyToAdd);

        _dbContext.Currencies.Add(currencyRow);
    }

    public async Task UpdateAsync(Currency updatedCurrency, CancellationToken ct = default)
    {
        if (updatedCurrency is null) throw new ArgumentNullException(nameof(updatedCurrency));

        var normalizedCode = NormalizeCode(updatedCurrency.Code);
        var existingCurrencyRow = await _dbContext.Currencies
            .SingleOrDefaultAsync(x => x.Code == normalizedCode, ct);

        if (existingCurrencyRow is null)
            throw new InvalidOperationException("Currency not found.");

        MapOntoEf(existingCurrencyRow, updatedCurrency);
    }

    public async Task<Currency?> GetByCodeAsync(string currencyCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(currencyCode)) 
            throw new ArgumentException("Code is required.", nameof(currencyCode));

        var normalizedCode = NormalizeCode(currencyCode);

        var currencyRow = await _dbContext.Currencies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Code == normalizedCode, ct);

        return currencyRow is null ? null : MapToDomain(currencyRow);
    }

    public async Task<IReadOnlyList<Currency>> ListAsync(CancellationToken ct = default)
    {
        var currencyRows = await _dbContext.Currencies
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(ct);

        return currencyRows.Select(MapToDomain).ToList();
    }

    private static string NormalizeCode(string codeInput) => codeInput.Trim().ToUpperInvariant();

    private static EfCurrency MapToEf(Currency currency)
    {
        return new EfCurrency
        {
            Code = NormalizeCode(currency.Code),
            Name = currency.Name,
            ExchangeRateToBase = currency.ExchangeRateToBase,
            IsActive = currency.IsActive
        };
    }

    private static void MapOntoEf(EfCurrency currencyRow, Currency currency)
    {
        currencyRow.Name = currency.Name;
        currencyRow.ExchangeRateToBase = currency.ExchangeRateToBase;
        currencyRow.IsActive = currency.IsActive;
    }

    private static Currency MapToDomain(EfCurrency currencyRow)
    {
        return Currency.Create(
            code: currencyRow.Code,
            name: currencyRow.Name,
            exchangeRateToBase: currencyRow.ExchangeRateToBase,
            isActive: currencyRow.IsActive);
    }
}
