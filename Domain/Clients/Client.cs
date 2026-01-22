using Domain.Common;
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

    private Client(Guid id, ClientType type, string name, IdentificationNumber identifier)
    {
        Id = id;
        Type = type;
        Name = name;
        Identifier = identifier;
    }

    public static Result<Client> Create(
        ClientType type,
        string name,
        IdentificationNumber identifier)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Client>.Fail(ErrorType.Validation, "Name required");

        return Result<Client>.Ok(
            new Client(Guid.NewGuid(), type, name, identifier));
    }

    public Result Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return Result.Fail(ErrorType.Validation, "Invalid name");

        Name = newName;
        return Result.Ok();
    }
}

