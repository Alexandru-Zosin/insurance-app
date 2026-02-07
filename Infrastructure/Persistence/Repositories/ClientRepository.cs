using Application.Common;
using Application.Repositories;
using Domain.Clients;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfClient = Infrastructure.Persistence.Models.Client;

namespace Infrastructure.Persistence.Repositories;

public sealed class ClientRepository(InsuranceDbContext _db) : IClientRepository
{
    public async Task AddAsync(Client client, CancellationToken ct = default)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));

        var ef = ToEfModel(client);

        _db.Clients.Add(ef);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default)
    {
        if (clientId == Guid.Empty) throw new ArgumentException("Client id is required.", nameof(clientId));

        var ef = await _db.Clients
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ClientKey == clientId, ct)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<Client>> SearchAsync(string? identifier, string? name, PageRequest pageRequest, CancellationToken ct = default)
    {
        if (pageRequest is null) throw new ArgumentNullException(nameof(pageRequest));
        if (pageRequest.PageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageRequest.PageNumber));
        if (pageRequest.PageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageRequest.PageSize));

        var q = _db.Clients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(identifier))
        {
            var id = identifier.Trim();
            q = q.Where(x => x.IdentificationNumber.Contains(id));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var n = name.Trim();
            q = q.Where(x => x.Name.Contains(n));
        }

        var skip = (pageRequest.PageNumber - 1) * pageRequest.PageSize;

        var rows = await q
            .OrderBy(x => x.Name)
            .ThenBy(x => x.IdentificationNumber)
            .Skip(skip)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    public async Task UpdateAsync(Client client, CancellationToken ct = default)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));

        var ef = await _db.Clients
            .SingleOrDefaultAsync(x => x.ClientKey == client.Id, ct)
            .ConfigureAwait(false);

        if (ef is null)
            throw new InvalidOperationException("Client not found.");

        UpdateEfModel(ef, client);

        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static EfClient ToEfModel(Client domain)
    {
        return new EfClient
        {
            ClientKey = domain.Id,
            ClientType = domain.Type.ToString(),
            Name = domain.Name,
            IdentificationNumber = domain.Identifier.Value,
            Email = domain.ContactInfo.Email,
            Phone = domain.ContactInfo.Phone,
            Street = domain.Address?.Street,
            Number = domain.Address?.Number
        };
    }

    private static void UpdateEfModel(EfClient ef, Client domain)
    {
        ef.ClientType = domain.Type.ToString();
        ef.Name = domain.Name;
        ef.IdentificationNumber = domain.Identifier.Value;
        ef.Email = domain.ContactInfo.Email;
        ef.Phone = domain.ContactInfo.Phone;
        ef.Street = domain.Address?.Street;
        ef.Number = domain.Address?.Number;
    }

    private static Client ToDomain(EfClient ef)
    {
        return Client.Rehydrate(
            id: ef.ClientKey,
            type: ParseClientType(ef.ClientType),
            name: ef.Name,
            identifier: IdentificationNumber.Create(ef.IdentificationNumber),
            contactInfo: ContactInfo.Create(ef.Email, ef.Phone),
            address: Address.CreateOptional(ef.Street, ef.Number));
    }

    private static ClientType ParseClientType(string value)
    {
        if (Enum.TryParse<ClientType>(value, ignoreCase: true, out var parsed))
            return parsed;

        throw new InvalidOperationException($"Unknown client type '{value}'.");
    }
}
