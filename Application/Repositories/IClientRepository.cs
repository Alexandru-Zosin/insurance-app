using Application.Common;
using Domain.Clients;
namespace Application.Repositories;

public interface IClientRepository
{
    void Add(Client client);
    Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default);
    Task<IReadOnlyList<Client>> ListAsync(string? identifier, string? name,
        PageRequest pageRequest, CancellationToken ct = default);
    Task UpdateAsync(Client client, CancellationToken ct = default);
}