using Application.Common;
using Application.Exceptions;
using Application.Repositories;
using Application.Repositories.SearchCriteria;
using Application.Services.Clients.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.ClientDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Clients;

namespace Application.Services.Clients;

public sealed class ClientService(
    IClientRepository clientRepository,
    IBuildingRepository buildingRepository,
    IPolicyRepository policyRepository,
    IUnitOfWork uow
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

        clientRepository.Add(newClient);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DuplicateKeyException)
        {
            return Result<CreateClientResponse>.Fail(ErrorType.Conflict, 
                "Identification number already exists.");
        }

        var response = new CreateClientResponse(newClient.Id);
        return Result<CreateClientResponse>.Ok(response);
    }

    public async Task<Result<GetClientDetailsResponse>> GetClientDetailsAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await clientRepository.GetByIdAsync(clientId, ct);
        if (client == null)
        {
            return Result<GetClientDetailsResponse>.Fail(
                ErrorType.NotFound,
                "Client not found");
        }

        var clientBuildings = await buildingRepository.ListByClientIdAsync(clientId, ct);
        var clientPolicies = await policyRepository.ListAsync(
            PolicySearchCriteria.ByClientId(clientId), ct: ct);

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

        var matchedClients = await clientRepository.ListAsync(
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
        var client = await clientRepository.GetByIdAsync(clientId, ct);
        if (client == null)
            return Result<UpdateClientResponse>.Fail(ErrorType.NotFound, "Client not found");

        var updatedName = request.ClientInfo.Name;
        var updatedAddress = request.ClientInfo.Address?.MapToDomain();
        var updatedContactInfo = request.ClientInfo.ContactInfo.MapToDomain();
        var updatedIdentificationNumber = request.ClientInfo.IdentificationNumber.MapToDomain();

        var originalIdentificationNumber = client.Identifier;
        if (originalIdentificationNumber != updatedIdentificationNumber)
        {
            uow.EnqueueAudit(new AuditEntry(
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

        await clientRepository.UpdateAsync(updatedClient, ct);
        await uow.SaveChangesAsync(ct);

        var response = new UpdateClientResponse(true);
        return Result<UpdateClientResponse>.Ok(response);
    }
}