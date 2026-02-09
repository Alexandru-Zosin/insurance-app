using Application.Common;
using Application.Repositories;
using Application.Services.Clients.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Clients;

namespace Application.Services.Clients;

public sealed class ClientService(
    IClientRepository _clientRepository,
    IBuildingRepository _buildingRepository,
    IPolicyRepository _policyRepository,
    IUnitOfWork _uow
    ) : IClientService
{
    public async Task<Result<CreateClientResponse>> CreateClientAsync(
        CreateClientRequest request,
        CancellationToken ct = default)
    {
        var clientIdentifier = request.Client.IdentificationNumber.MapToDomain();
        var clientContactInfo = request.Client.ContactInfo.MapToDomain();
        var clientAddress = request.Client.Address?.MapToDomain();

        var newClient = Client.Create(
            request.Client.Type,
            request.Client.Name,
            clientIdentifier,
            clientContactInfo,
            clientAddress);

        _clientRepository.Add(newClient, ct);

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

        var response = new CreateClientResponse(newClient.Id);
        return Result<CreateClientResponse>.Ok(response);
    }

    public async Task<Result<GetClientDetailsResponse>> GetClientDetailsAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await _clientRepository.GetByIdAsync(clientId, ct);
        if (client == null)
        {
            return Result<GetClientDetailsResponse>.Fail(
                ErrorType.NotFound,
                "Client not found");
        }

        var clientBuildings = await _buildingRepository.GetByClientIdAsync(clientId, ct);
        var clientPolicies = await _policyRepository.GetByClientIdAsync(clientId, ct);

        var response = new GetClientDetailsResponse(
            ClientDetailedDto.From(
                    client,
                    clientBuildings.Select(BuildingListItemDto.From).ToArray(),
                    clientPolicies.Select(PolicyListItemDto.From).ToArray()
                ));
        return Result<GetClientDetailsResponse>.Ok(response);
    }

    public async Task<Result<SearchClientsResponse>> SearchClientsAsync(
        SearchClientsRequest request, CancellationToken ct = default)
    {
        var page = request.PageRequest;

        var matchedClients = await _clientRepository.SearchAsync(
            request.Identifier,
            request.Name,
            page,
            ct);

        var response = new SearchClientsResponse(matchedClients.Select(ClientListItemDto.From).ToArray());
        return Result<SearchClientsResponse>.Ok(response);
    }
    public async Task<Result<UpdateClientResponse>> UpdateClientAsync(
        Guid clientId,
        UpdateClientRequest request,
        CancellationToken ct = default)
    {
        var client = await _clientRepository.GetByIdAsync(clientId, ct);
        if (client == null)
            return Result<UpdateClientResponse>.Fail(ErrorType.NotFound, "Client not found");

        var updatedName = request.ClientInfo.Name;
        var updatedAddress = request.ClientInfo.Address?.MapToDomain();
        var updatedContactInfo = request.ClientInfo.ContactInfo.MapToDomain();
        var updatedIdentificationNumber = request.ClientInfo.IdentificationNumber.MapToDomain();

        var originalIdentificationNumber = client.Identifier;
        if (originalIdentificationNumber != updatedIdentificationNumber)
        {
            _uow.EnqueueAudit(new AuditEntry(
                EntityType: nameof(Client),
                EntityId: client.Id,
                Action: "ChangeIdentificationNumber",
                OldValue: originalIdentificationNumber.Value,
                NewValue: updatedIdentificationNumber.Value,
                PerformedBy: request.PerformedByBrokerId,
                PerformedAtUtc: DateTime.UtcNow));
        }

        var updatedClient = client.UpdateName(updatedName)
                                  .UpdateContactInfo(updatedContactInfo)
                                  .UpdateAddress(updatedAddress);

        await _clientRepository.UpdateAsync(updatedClient, ct);
        await _uow.SaveChangesAsync(ct);

        var response = new UpdateClientResponse(true);
        return Result<UpdateClientResponse>.Ok(response);
    }
}