using Application.Common;
using Domain.Brokers;
namespace Application.Repositories;

public interface IBrokerRepository
{
    void Add(Broker broker);
    Task UpdateAsync(Broker broker, CancellationToken ct = default);
    Task<Broker?> GetByIdAsync(Guid brokerId, CancellationToken ct = default);
    Task<Broker?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Broker>> ListAsync(PageRequest pageRequest,
                                          CancellationToken ct = default);
}