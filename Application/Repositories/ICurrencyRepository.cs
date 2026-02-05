using Domain.Currencies;

namespace Application.Repositories;

public interface ICurrencyRepository
{
    Task AddAsync(Currency aggregate, CancellationToken ct = default);
    Task UpdateAsync(Currency aggregate, CancellationToken ct = default);
    Task<Currency?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Currency>> ListAsync(CancellationToken ct = default);
}
