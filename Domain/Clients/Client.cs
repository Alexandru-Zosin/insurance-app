using Domain.Common;
using Domain.Shared;
namespace Domain.Clients;
public sealed class Client
{
    public Guid Id { get; }
    public ClientType Type { get; }
    public string Name { get; private set; }
    public IdentificationNumber Identifier { get; }
    public ContactInfo ContactInfo { get; private set; }
    public Address? Address { get; private set; }

    private Client(Guid id, ClientType type, string name, IdentificationNumber identifier,
        ContactInfo contactInfo, Address? address)
    {
        Id = id;
        Type = type;
        Name = name;
        Identifier = identifier;
        ContactInfo = contactInfo;
        Address = address;

        ValidateInvariants();
    }

    public static Client Create(
        ClientType type,
        string name,
        IdentificationNumber identifier,
        ContactInfo contactInfo,
        Address? address)
    {
        return new Client(Guid.NewGuid(), type, name, identifier, contactInfo,
                address);
    }

    public static Client Rehydrate(
    Guid id,
    ClientType type,
    string name,
    IdentificationNumber identifier,
    ContactInfo contactInfo,
    Address? address)
    {
        return new Client(id, type, name, identifier, contactInfo, address);
    }

    public Client UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        return this;
    }

    public Client UpdateContactInfo(ContactInfo contactInfo)
    {
        ValidateContactInfo(contactInfo);
        ContactInfo = contactInfo;
        return this;
    }
    public Client UpdateAddress(Address? address)
    {
        Address = address;
        return this;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Invalid Client Name.");
    }

    private static void ValidateContactInfo(ContactInfo contactInfo)
    {
        if (contactInfo is null)
            throw new DomainException("Invalid ContactInfo.");
    }

    private void ValidateInvariants()
    {
        ValidateName(Name);
        ValidateContactInfo(ContactInfo);
    }
}

