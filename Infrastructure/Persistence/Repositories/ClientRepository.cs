using Application.Common;
using Application.Repositories;
using Domain.Clients;
using Domain.Shared;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfClient = Infrastructure.Persistence.Models.Client;

namespace Infrastructure.Persistence.Repositories;

public sealed class ClientRepository(InsuranceDbContext _dbContext) : IClientRepository
{
    public void Add(Client clientToAdd, CancellationToken ct = default)
    {
        var clientRow = MapToEf(clientToAdd);
        _dbContext.Clients.Add(clientRow);
    }

    public async Task<Client?> GetByIdAsync(Guid clientId, CancellationToken ct = default)
    {
        var clientRow = await _dbContext.Clients
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ClientKey == clientId, ct);

        return clientRow is null ? null : MapToDomain(clientRow);
    }

    public async Task<IReadOnlyList<Client>> SearchAsync(string? identifierFilter, string? nameFilter, PageRequest page, CancellationToken ct = default)
    {
        var query = _dbContext.Clients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(identifierFilter))
        {
            var idFilterValue = identifierFilter.Trim();
            query = query.Where(x => x.IdentificationNumber.Contains(idFilterValue));
        }

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            var nameValue = nameFilter.Trim();
            query = query.Where(x => x.Name.Contains(nameValue));
        }

        var clientRows = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.IdentificationNumber)
            .Skip(page.Offset)
            .Take(page.Take)
            .ToListAsync(ct);

        return clientRows.Select(MapToDomain).ToList();
    }

    public async Task UpdateAsync(Client updatedClient, CancellationToken ct = default)
    {
        var existingClientRow = await _dbContext.Clients
                .SingleOrDefaultAsync(x => x.ClientKey == updatedClient.Id, ct);

        if (existingClientRow is null)
            throw new InvalidOperationException("Client not found.");

        MapOntoEf(existingClientRow, updatedClient);
    }

    private static EfClient MapToEf(Client client)
    {
        return new EfClient
        {
            ClientKey = client.Id,
            ClientType = client.Type.ToString(),
            Name = client.Name,
            IdentificationNumber = client.Identifier.Value,
            Email = client.ContactInfo.Email,
            Phone = client.ContactInfo.Phone,
            Street = client.Address?.Street,
            Number = client.Address?.Number
        };
    }

    private static void MapOntoEf(EfClient clientRow, Client client)
    {
        clientRow.ClientType = client.Type.ToString();
        clientRow.Name = client.Name;
        clientRow.IdentificationNumber = client.Identifier.Value;
        clientRow.Email = client.ContactInfo.Email;
        clientRow.Phone = client.ContactInfo.Phone;
        clientRow.Street = client.Address?.Street;
        clientRow.Number = client.Address?.Number;
    }

    private static Client MapToDomain(EfClient clientRow)
    {
        return Client.Rehydrate(
            id: clientRow.ClientKey,
            type: ParseClientType(clientRow.ClientType),
            name: clientRow.Name,
            identifier: IdentificationNumber.Create(clientRow.IdentificationNumber),
            contactInfo: ContactInfo.Create(clientRow.Email, clientRow.Phone),
            address: Address.CreateOptional(clientRow.Street, clientRow.Number));
    }

    private static ClientType ParseClientType(string clientTypeValue)
    {
        if (Enum.TryParse<ClientType>(clientTypeValue, ignoreCase: true, out var clientType))
            return clientType;

        throw new InvalidOperationException($"Unknown client type '{clientTypeValue}'.");
    }
}
