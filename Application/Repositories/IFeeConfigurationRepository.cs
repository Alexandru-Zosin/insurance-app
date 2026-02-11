using Domain.Configurations;
namespace Application.Repositories;

public interface IFeeConfigurationRepository
{
    void Add(FeeConfiguration feeConfiguration, CancellationToken ct = default);
    Task<FeeConfiguration?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FeeConfiguration>> ListAsync(CancellationToken ct = default);
    Task UpdateAsync(FeeConfiguration feeConfiguration, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
}
