using Application.Common;
using Application.Services.Clients.DTO;
using Domain.Clients;
using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class CreateClientService
    : IUseCase<
        CreateClientRequest,
         Result<CreateClientResponse>>
{
    private readonly IClientRepository _clients;

    public CreateClientService(IClientRepository clients)
    {
        _clients = clients;
    }
    public async Task<Result<CreateClientResponse>> HandleAsync(
        CreateClientRequest request,
        CancellationToken ct = default)
    {
        var existing = await _clients.GetByRegistrationNumberAsync(
            request.RegistrationNumber,
            ct);

        if (existing != null)
        {
            return Result<CreateClientResponse>.Fail(
                ErrorType.Conflict,
                "Identification number already exists");
        }

        var idResult = IdentificationNumber.Create(request.RegistrationNumber);
        if (!idResult.IsSuccess)
        {
            return Result<CreateClientResponse>.Fail(
                idResult.ErrorType,
                idResult.ErrorMessage);
        }

        var contactResult = ContactInfo.Create(request.Email, request.Phone);
        if (!contactResult.IsSuccess)
        {
            return Result<CreateClientResponse>.Fail(
                contactResult.ErrorType,
                contactResult.ErrorMessage);
        }

        var addressResult = Address.Create(request.Street, request.Number);
        if (!addressResult.IsSuccess)
        {
            return Result<CreateClientResponse>.Fail(
                addressResult.ErrorType,
                addressResult.ErrorMessage);
        }

        //Address? address = null;
        //if (request.Street != null || request.Number != null)
        //{
        //    var addressResult = Address.Create(request.Street ?? string.Empty, request.Number ?? string.Empty);
        //    if (!addressResult.IsSuccess)
        //    {
        //        return Result<CreateClientResponse>.Fail(
        //            addressResult.ErrorType,
        //            addressResult.ErrorMessage);
        //    }

        //    address = addressResult.Value!;
        //}

        var clientResult = Client.Create(
            Enum.Parse<ClientType>(request.ClientType),
            request.Name,
            idResult.Value!,
            contactResult.Value!,
            addressResult.Value!);

        if (!clientResult.IsSuccess)
        {
            return Result<CreateClientResponse>.Fail(
                clientResult.ErrorType,
                clientResult.ErrorMessage);
        }

        await _clients.AddAsync(clientResult.Value!, ct);

        return Result<CreateClientResponse>.Ok(
            new CreateClientResponse(clientResult.Value!.Id));
    }
}
