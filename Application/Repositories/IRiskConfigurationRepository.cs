using Domain.Configurations;
namespace Application.Repositories;
public interface IRiskConfigurationRepository
{
    void Add(IRiskConfiguration riskConfiguration);
    Task UpdateAsync(IRiskConfiguration riskConfiguration, CancellationToken ct = default);
    Task<IRiskConfiguration?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<IRiskConfiguration>> ListAsync(CancellationToken ct = default);
}
