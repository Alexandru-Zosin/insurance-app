using Domain.Clients;
using Domain.Shared;
using Domain.ValueObjects;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly InsuranceDbContext _db;

    public ClientRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public async Task<Domain.Clients.Client?> GetByIdAsync(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Clients
            .AsNoTracking()
            .SingleOrDefaultAsync(
                c => c.ClientKey == clientId,
                cancellationToken)
            .ConfigureAwait(false);

        return ef == null ? null : Map(ef);
    }

    public async Task<Domain.Clients.Client?> GetByRegistrationNumberAsync(
        string registrationNumber,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Clients
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.RegistrationNumber == registrationNumber,
                cancellationToken)
            .ConfigureAwait(false);

        return ef == null ? null : Map(ef);
    }

    public async Task<IReadOnlyList<Client>> SearchByNameAsync(
    string name,
    CancellationToken cancellationToken)
    {
        var entities = await _db.Clients
            .AsNoTracking()
            .Where(c => c.Name.Contains(name))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return entities
            .Select(Map)
            .ToList();
    }

    public async Task AddAsync(
        Client client,
        CancellationToken cancellationToken)
    {
        await _db.Clients.AddAsync(
            new Models.Client
            {
                ClientKey = client.Id,
                ClientType = client.Type.ToString(),
                Name = client.Name,
                RegistrationNumber = client.Identifier.Value,
                Email = client.ContactInfo.Email,
                Phone = client.ContactInfo.Phone,
                Address = $"{client.Address!.Street} {client.Address.Number}"
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task UpdateAsync(
        Client client,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Clients
            .SingleAsync(
                c => c.ClientKey == client.Id,
                cancellationToken)
            .ConfigureAwait(false);

        ef.Name = client.Name;
        ef.Email = client.ContactInfo.Email;
        ef.Phone = client.ContactInfo.Phone;
        ef.Address = $"{client.Address!.Street} {client.Address.Number}";
    }

    private static Client Map(Models.Client ef)
    {
        var identifier = IdentificationNumber.Create(ef.RegistrationNumber).Value!;
        var contact = ContactInfo.Create(ef.Email, ef.Phone).Value!;
        var address = Address.Create(ef.Address, "").Value!;

        return Client.Create(
            Enum.Parse<ClientType>(ef.ClientType),
            ef.Name,
            identifier,
            contact,
            address).Value!;
    }
}
