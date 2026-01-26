using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
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
    public Address Address { get; private set; }


    private Client(Guid id, ClientType type, string name, IdentificationNumber identifier,
        ContactInfo contactInfo,
        Address address)
    {
        Id = id;
        Type = type;
        Name = name;
        Identifier = identifier;
        ContactInfo = contactInfo;
        Address = address;
    }

    public static Result<Client> Create(
        ClientType type,
        string name,
        IdentificationNumber identifier,
        ContactInfo contactInfo,
        Address address)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Client>.Fail(ErrorType.Validation, "Name required");

        return Result<Client>.Ok(
            new Client(Guid.NewGuid(), type, name, identifier, contactInfo,
                address));
    }

    public Result<bool> ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return Result<bool>.Fail(ErrorType.Validation, "Invalid name");

        Name = newName;
        return Result<bool>.Ok(true);
    }

    public Result<bool> ChangeContactInfo(ContactInfo contactInfo)
    {
        ContactInfo = contactInfo;
        return Result<bool>.Ok(true);
    }

    public Result<bool> ChangeAddress(Address address)
    {
        Address = address;
        return Result<bool>.Ok(true);
    }
}

