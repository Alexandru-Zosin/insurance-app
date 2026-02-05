using Application.Common;
using Domain.Brokers;
namespace Application.Repositories;

public interface IBrokerRepository
{
    Task AddAsync(Broker broker, CancellationToken cancellationToken = default);
    Task UpdateAsync(Broker broker, CancellationToken cancellationToken = default);
    Task<Broker?> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default);
    Task<Broker?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Broker>> ListAsync(PageRequest pageRequest,
                                          CancellationToken ct = default);
}