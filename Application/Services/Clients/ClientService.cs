using Application.Common;
using Application.Repositories;
using Application.Services.Clients.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Clients;
using System.Linq;

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
        var identifier = request.Client.IdentificationNumber.ToDomain();
        var contact = request.Client.ContactInfo.ToDomain();
        var address = request.Client.Address.ToDomain();

        var client = Client.Create(
            request.Client.Type,
            request.Client.Name,
            identifier,
            contact,
            address);

        await _clients.AddAsync(client, ct);

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
            new CreateClientResponse(client.Id));
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
            ClientDetailedDto.From(
                    client,
                    buildings.Select(BuildingListItemDto.From).ToArray(),
                    policies.Select(PolicyListItemDto.From).ToArray()
                ));

        return Result<GetClientDetailsResponse>.Ok(response);
    }

    public async Task<Result<SearchClientsResponse>> SearchClientsAsync(
        SearchClientsRequest request, CancellationToken ct = default)
    {
        var page = request.PageRequest;

        var searchResult = await _clients.SearchAsync(
            request.Identifier,
            request.Name,
            page,
            ct);

        var response = new SearchClientsResponse(searchResult.Select(ClientListItemDto.From).ToArray());
        return Result<SearchClientsResponse>.Ok(response);
    }
    public async Task<Result<UpdateClientResponse>> UpdateClientAsync(
        UpdateClientRequest request,
        CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.ClientId, ct);
        if (client == null)
        {
            return Result<UpdateClientResponse>.Fail(
                ErrorType.NotFound, "Client not found");
        }

        var newAddress = request.ClientInfo.Address.ToDomain();
        var newContactInfo = request.ClientInfo.ContactInfo.ToDomain();

        var updatedClient = client.UpdateName(request.ClientInfo.Name)
                              .UpdateContactInfo(newContactInfo)
                              .UpdateAddress(newAddress);

        await _clients.UpdateAsync(updatedClient, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateClientResponse>.Ok(
            new UpdateClientResponse(true));
    }
}