using Domain.Shared;
namespace Domain.Clients;

public enum ClientType
{
    Individual,
    Company
}

public sealed class Client
{
    public Guid Id { get; }
    public ClientType Type { get; }
    public string Name { get; private set; }
    public IdentificationNumber Identifier { get; }
    public ContactInfo ContactInfo { get; private set; }
    public Address? Address { get; private set; }

    private Client(Guid id, ClientType type, string name, IdentificationNumber identifier,
        ContactInfo contactInfo,
        Address? address)
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

    public Client ChangeName(string newName)
    {
        Name = newName;
        return this;
    }

    public Client ChangeContactInfo(ContactInfo contactInfo)
    {
        ContactInfo = contactInfo;
        return this;
    }
    public Client ChangeAddress(Address? address)
    {
        Address = address;
        return this;
    }

    private void ValidateInvariants()
    {
    }
}

