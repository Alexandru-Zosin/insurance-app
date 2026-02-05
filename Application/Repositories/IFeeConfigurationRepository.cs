using Application.Common;
using Domain.Configurations;

namespace Application.Repositories;

public interface IFeeConfigurationRepository
{
    Task AddAsync(FeeConfiguration aggregate, CancellationToken ct = default);
    Task UpdateAsync(FeeConfiguration aggregate,CancellationToken ct = default);
    Task<FeeConfiguration?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FeeConfiguration>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<FeeConfiguration>> ListAsync(PageRequest pageRequest, CancellationToken ct = default);
}
