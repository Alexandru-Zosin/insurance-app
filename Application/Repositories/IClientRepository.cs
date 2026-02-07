using Application.Common;
using Domain.Clients;
namespace Application.Repositories;

public interface IClientRepository
{
    Task AddAsync(Client client, CancellationToken ct = default);
    Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default);
    Task<IReadOnlyList<Client>> SearchAsync(string? identifier, string? name,
        PageRequest pageRequest, CancellationToken ct = default);
    Task UpdateAsync(Client client, CancellationToken ct = default);
}