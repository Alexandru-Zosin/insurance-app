using Application.Common;
using Domain.Configurations;

namespace Application.Repositories;

public interface IRiskFactorRepository
{
    Task AddAsync<T>(T cfg, CancellationToken ct = default)
        where T : RiskFactorConfiguration<T>;

    Task UpdateAsync<T>(T cfg, CancellationToken ct = default)
        where T : RiskFactorConfiguration<T>;

    Task<T?> GetByIdAsync<T>(Guid id, CancellationToken ct = default)
        where T : RiskFactorConfiguration<T>;
}
