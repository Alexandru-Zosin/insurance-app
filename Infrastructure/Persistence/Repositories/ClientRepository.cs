using Application.Common;
using Application.Repositories;
using Domain.Clients;
using Domain.Shared;
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

    public async Task<Client?> GetByIdAsync(
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

    public async Task<IReadOnlyList<Client>> SearchAsync(
    string? registrationNumber,
    string? name,
    PageRequest page,
    CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(registrationNumber))
        {
            var result = await _db.Clients
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                c => c.RegistrationNumber == registrationNumber
                            );

            return result == null ? Array.Empty<Client>() : new[] { Map(result) }; 
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var result = await _db.Clients
                                .AsNoTracking()
                                .Where(c => c.Name.Contains(name))
                                .OrderBy(c => c.ClientId)
                                .Skip(page.Skip)
                                .Take(page.Take)
                                .ToListAsync();

            return result.Select(Map).ToList();
        }

        return Array.Empty<Client>();
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
                Street = client.Address?.Street,
                Number = client.Address?.Number
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    public async Task UpdateAsync(
        Client updatedClient,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Clients
            .SingleAsync(
                c => c.ClientKey == updatedClient.Id,
                cancellationToken)
            .ConfigureAwait(false);

        ef.Name = updatedClient.Name;
        ef.Email = updatedClient.ContactInfo.Email;
        ef.Phone = updatedClient.ContactInfo.Phone;
        ef.Street = updatedClient.Address?.Street;
        ef.Number = updatedClient.Address?.Number;
    }

    private static Client Map(Models.Client ef)
    {
        var identifier = IdentificationNumber.Create(ef.RegistrationNumber);
        var contact = ContactInfo.Create(ef.Email, ef.Phone);
        var address = Address.CreateOptional(ef.Street, ef.Number);

        return Client.Rehydrate(
        ef.ClientKey,
        Enum.Parse<ClientType>(ef.ClientType),
        ef.Name,
        identifier,
        contact,
        address
        );
    }
}
