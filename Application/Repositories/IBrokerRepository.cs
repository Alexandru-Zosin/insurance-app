using Domain.Brokers;
namespace Application.Repositories;

public interface IBrokerRepository
{
    Task<Broker?> GetByIdAsync(Guid brokerId, CancellationToken cancellationToken = default);
    Task AddAsync(Broker broker, CancellationToken cancellationToken = default);
    Task UpdateAsync(Broker broker, CancellationToken cancellationToken = default);
}