using Application.Common;
using Application.Services.Clients.DTOs;
using Domain.Buildings;
using Domain.Clients;
using Domain.Common;
using Domain.Policies;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Services.Clients;

public sealed class ClientService(
    IClientRepository _clients,
    IBuildingRepository _buildings,
    IPolicyRepository _policies,
    IUnitOfWork _uow
    ) : IClientService
{
    public async Task<Result<CreateClientResponse>> CreateClientAsync(
        CreateClientRequest request,
        CancellationToken ct = default)
    {
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

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result<CreateClientResponse>.Fail(
                ErrorType.Conflict,
                "Identification number already exists.");
        }

        return Result<CreateClientResponse>.Ok(
            new CreateClientResponse(clientResult.Value!.Id));
    }

    public async Task<Result<GetClientDetailsResponse>> GetClientDetailsAsync(
        GetClientDetailsRequest request,
        CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.ClientId, ct);
        if (client == null)
        {
            return Result<GetClientDetailsResponse>.Fail(
                ErrorType.NotFound,
                "Client not found");
        }

        var buildings = await _buildings.GetByClientIdAsync(request.ClientId, ct);
        var policies = await _policies.GetByClientIdAsync(request.ClientId, ct);

        var response = new GetClientDetailsResponse(
            ClientDto.From(client),
            buildings.Select(BuildingDto.From).ToList(),
            policies.Select(PolicyDto.From).ToList());

        return Result<GetClientDetailsResponse>.Ok(response);
    }

    public async Task<Result<SearchClientsResponse>> SearchClientsAsync(
        SearchClientsRequest request, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(request.Identifier))
        {
            var client = await _clients.GetByRegistrationNumberAsync(
                request.Identifier,
                ct);

            return Result<SearchClientsResponse>.Ok(
                new SearchClientsResponse(
                    client == null
                        ? Array.Empty<ClientSearchResultDto>()
                        : new[] { ClientSearchResultDto.From(client) }));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var results = await _clients.SearchByNameAsync(
                request.Name,
                ct);

            return Result<SearchClientsResponse>.Ok(
                new SearchClientsResponse(
                    results.Select(ClientSearchResultDto.From).ToList()));
        }
        
        return Result<SearchClientsResponse>.Ok(
           new SearchClientsResponse(
               Array.Empty<ClientSearchResultDto>()));
    }

    public async Task<Result<UpdateClientResponse>> UpdateClientAsync(
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
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateClientResponse>.Ok(
            new UpdateClientResponse(true));
    }
}
