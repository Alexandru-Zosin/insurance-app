using Application.Common;
using Domain.Clients;
using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class UpdateClientService
    : IUseCase<
        UpdateClientService.Request,
        Result<UpdateClientService.Response>>
{
    public sealed record Request(
        Guid ClientId,
        string Name,
        string Email,
        string Phone,
        string? Address);

    public sealed record Response(bool Updated);

    private readonly IClientRepository _clients;

    public UpdateClientService(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<Result<Response>> HandleAsync(
        Request request,
        CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.ClientId, ct);
        if (client == null)
        {
            return Result<Response>.Fail(
                ErrorType.NotFound,
                "Client not found");
        }


        var nameResult = client.ChangeName(request.Name);
        if (!nameResult.IsSuccess)
        {
            return Result<Response>.Fail(
                nameResult.ErrorType,
                nameResult.ErrorMessage);
        }

        var contactResult = ContactInfo.Create(request.Email, request.Phone);
        if (!contactResult.IsSuccess)
        {
            return Result<Response>.Fail(
                contactResult.ErrorType,
                contactResult.ErrorMessage);
        }

        var contactUpdateResult = client.ChangeContactInfo(contactResult.Value!);
        if (!contactUpdateResult.IsSuccess)
        {
            return Result<Response>.Fail(
                contactUpdateResult.ErrorType,
                contactUpdateResult.ErrorMessage);
        }

        if (request.Address != null)
        {
            var addressResult = Address.Create(request.Address, "");
            if (!addressResult.IsSuccess)
            {
                return Result<Response>.Fail(
                    addressResult.ErrorType,
                    addressResult.ErrorMessage);
            }

            var addressUpdateResult = client.ChangeAddress(addressResult.Value!);
            if (!addressUpdateResult.IsSuccess)
            {
                return Result<Response>.Fail(
                    addressUpdateResult.ErrorType,
                    addressUpdateResult.ErrorMessage);
            }
        }

        await _clients.UpdateAsync(client, ct);

        return Result<Response>.Ok(
            new Response(true));
    }
}
