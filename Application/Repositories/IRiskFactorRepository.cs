namespace Application.Repositories;

public interface IRiskFactorRepository
{
    Task AddAsync(RiskFactorConfiguration aggregate,
                                            CancellationToken ct = default);
    Task UpdateAsync(RiskFactorConfiguration aggregate,
                                            CancellationToken ct = default);
    Task<RiskFactorConfiguration?> GetByIdAsync(Guid id,
                                                CancellationToken ct = default);
    Task<IReadOnlyList<RiskFactorConfiguration>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RiskFactorConfiguration>> ListAsync(CancellationToken ct = default);
}
