using Application.Common;
using Application.Services.Clients.DTO;
using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class UpdateClientService
    : IUseCase<
        UpdateClientRequest,
        Result<UpdateClientResponse>>
{

    private readonly IClientRepository _clients;

    public UpdateClientService(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<Result<UpdateClientResponse>> HandleAsync(
        UpdateClientRequest request,
        CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.ClientId, ct);
        if (client == null)
        {
            return Result<UpdateClientResponse>.Fail(
                ErrorType.NotFound,
                "Client not found");
        }


        var nameResult = client.ChangeName(request.Name);
        if (!nameResult.IsSuccess)
        {
            return Result<UpdateClientResponse>.Fail(
                nameResult.ErrorType,
                nameResult.ErrorMessage);
        }

        var contactResult = ContactInfo.Create(request.Email, request.Phone);
        if (!contactResult.IsSuccess)
        {
            return Result<UpdateClientResponse>.Fail(
                contactResult.ErrorType,
                contactResult.ErrorMessage);
        }

        var contactUpdateResult = client.ChangeContactInfo(contactResult.Value!);
        if (!contactUpdateResult.IsSuccess)
        {
            return Result<UpdateClientResponse>.Fail(
                contactUpdateResult.ErrorType,
                contactUpdateResult.ErrorMessage);
        }

        if (request.Street != null || request.Number != null)
        {
            var addressResult = Address.Create(request.Street ?? string.Empty, request.Number ?? string.Empty);
            if (!addressResult.IsSuccess)
            {
                return Result<UpdateClientResponse>.Fail(
                    addressResult.ErrorType,
                    addressResult.ErrorMessage);
            }

            var addressUpdateResult = client.ChangeAddress(addressResult.Value!);
            if (!addressUpdateResult.IsSuccess)
            {
                return Result<UpdateClientResponse>.Fail(
                    addressUpdateResult.ErrorType,
                    addressUpdateResult.ErrorMessage);
            }
        }

        await _clients.UpdateAsync(client, ct);

        return Result<UpdateClientResponse>.Ok(
            new UpdateClientResponse(true));
    }
}
