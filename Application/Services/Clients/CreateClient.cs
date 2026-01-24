using Application.Common;
using Domain.Clients;
using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class CreateClientService
    : IUseCase<
        CreateClientService.Request,
        Result<CreateClientService.Response>>
{
    public sealed record Request(
        string ClientType,
        string Name,
        string RegistrationNumber,
        string Email,
        string Phone,
        string? Address);

    public sealed record Response(Guid ClientId);

    private readonly IClientRepository _clients;

    public CreateClientService(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<Result<Response>> HandleAsync(
        Request request,
        CancellationToken ct = default)
    {
        var existing = await _clients.GetByRegistrationNumberAsync(
            request.RegistrationNumber,
            ct);

        if (existing != null)
        {
            return Result<Response>.Fail(
                ErrorType.Conflict,
                "Identification number already exists");
        }

        var idResult = IdentificationNumber.Create(request.RegistrationNumber);
        if (!idResult.IsSuccess)
        {
            return Result<Response>.Fail(
                idResult.ErrorType,
                idResult.ErrorMessage);
        }

        var contactResult = ContactInfo.Create(request.Email, request.Phone);
        if (!contactResult.IsSuccess)
        {
            return Result<Response>.Fail(
                contactResult.ErrorType,
                contactResult.ErrorMessage);
        }

        Address? address = null;
        if (request.Address != null)
        {
            var addressResult = Address.Create(request.Address, "");
            if (!addressResult.IsSuccess)
            {
                return Result<Response>.Fail(
                    addressResult.ErrorType,
                    addressResult.ErrorMessage);
            }

            address = addressResult.Value!;
        }

        var clientResult = Client.Create(
            Enum.Parse<ClientType>(request.ClientType),
            request.Name,
            idResult.Value!,
            contactResult.Value!,
            address);

        if (!clientResult.IsSuccess)
        {
            return Result<Response>.Fail(
                clientResult.ErrorType,
                clientResult.ErrorMessage);
        }

        await _clients.AddAsync(clientResult.Value!, ct);

        return Result<Response>.Ok(
            new Response(clientResult.Value!.Id));
    }
}
