using Domain.Currencies;

namespace Application.Repositories;

public interface ICurrencyRepository
{
    void Add(Currency currency);
    Task UpdateAsync(Currency currency, CancellationToken ct = default);
    Task<Currency?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Currency>> ListAsync(CancellationToken ct = default);
}
