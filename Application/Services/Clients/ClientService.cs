using Application.Common;
using Application.Repositories;
using Application.Services.Clients.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Clients;

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
        var address = request.Client.Address?.ToDomain();

        var client = Client.Create(
            request.Client.Type,
            request.Client.Name,
            identifier,
            contact,
            address);

        _clients.Add(client, ct);

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

        var response = new CreateClientResponse(client.Id);
        return Result<CreateClientResponse>.Ok(response);
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
        var pageRequest = request.PageRequest;

        var searchResult = await _clients.SearchAsync(
            request.Identifier,
            request.Name,
            pageRequest,
            ct);

        var response = new SearchClientsResponse(searchResult.Select(ClientListItemDto.From).ToArray());
        return Result<SearchClientsResponse>.Ok(response);
    }
    public async Task<Result<UpdateClientResponse>> UpdateClientAsync(
        Guid requestClientId,
        UpdateClientRequest request,
        CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(requestClientId, ct);
        if (client == null)
            return Result<UpdateClientResponse>.Fail(ErrorType.NotFound, "Client not found");

        var newName = request.ClientInfo.Name;
        var newAddress = request.ClientInfo.Address?.ToDomain();
        var newContactInfo = request.ClientInfo.ContactInfo.ToDomain();
        var newIdentificationNumber = request.ClientInfo.IdentificationNumber.ToDomain();

        var originalIdentificationNumber = client.Identifier;
        if (originalIdentificationNumber != newIdentificationNumber)
        {
            _uow.EnqueueAudit(new AuditEntry(
                EntityType: nameof(Client),
                EntityId: client.Id,
                Action: "ChangeIdentificationNumber",
                OldValue: originalIdentificationNumber.Value,
                NewValue: newIdentificationNumber.Value,
                PerformedBy: request.PerformedByBrokerId,
                PerformedAtUtc: DateTime.UtcNow));
        }

        var updatedClient = client.UpdateName(newName)
                                  .UpdateContactInfo(newContactInfo)
                                  .UpdateAddress(newAddress);

        await _clients.UpdateAsync(updatedClient, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateClientResponse(true);
        return Result<UpdateClientResponse>.Ok(response);
    }
}