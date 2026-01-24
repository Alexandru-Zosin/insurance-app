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

    public Domain.Clients.Client? GetById(Guid clientId)
    {
        var ef = _db.Clients
                .AsNoTracking()
                .SingleOrDefault(c => c.ClientKey == clientId);

        return ef == null ? null : Map(ef);
    }

    public Domain.Clients.Client? GetByRegistrationNumber(string registrationNumber)
    {
        var ef = _db.Clients
                .AsNoTracking()
                .SingleOrDefault(x => x.RegistrationNumber == registrationNumber);

        return ef == null ? null : Map(ef);
    }

    public IReadOnlyList<Client> SearchByName(string name)
    {
        return _db.Clients
            .AsNoTracking()
            .Where(c => c.Name.Contains(name))
            .Select(Map)
            .ToList();
    }

    public void Add(Client client)
    {
        _db.Clients.Add(new Models.Client
        {
            ClientKey = client.Id,
            ClientType = client.Type.ToString(),
            Name = client.Name,
            RegistrationNumber = client.Identifier.Value,
            Email = client.ContactInfo.Email,
            Phone = client.ContactInfo.Phone,
            Address = $"{client.Address!.Street} {client.Address.Number}"
        });
    }

    public void Update(Client client)
    {
        var ef = _db.Clients
            .Single(c => c.ClientKey == client.Id);

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
