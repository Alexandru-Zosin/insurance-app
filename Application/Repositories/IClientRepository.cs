using Domain.Clients;
namespace Application.Repositories;

public interface IClientRepository
{
    Task AddAsync(Client client, CancellationToken ct = default);
    Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default);
    Task<Client?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken ct = default);
    Task<IReadOnlyList<Client>> SearchByNameAsync(string name, CancellationToken ct = default);
    Task UpdateAsync(Client client, CancellationToken ct = default);
}