using Application.Common;
using Application.Repositories;
using Application.Services.Clients.DTOs;
using Domain.Clients;
using Domain.Shared;

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
        var identifier = IdentificationNumber.Create(request.RegistrationNumber);
        var contact = ContactInfo.Create(request.Email, request.Phone);
        var address = Address.CreateOptional(request.Street, request.Number);

        var client = Client.Create(
            Enum.Parse<ClientType>(request.ClientType),
            request.Name,
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
            ClientDto.From(client),
            buildings.Select(BuildingDto.From).ToList(),
            policies.Select(PolicyDto.From).ToList());

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

        return Result<SearchClientsResponse>.Ok(
            new SearchClientsResponse(
                searchResult.Select(ClientSearchResultDto.From).ToList()));
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

        var newAddress = Address.CreateOptional(request.Street, request.Number);
        var newContactInfo = ContactInfo.Create(request.Email, request.Phone);

        var updatedClient = client.ChangeName(request.Name)
                              .ChangeContactInfo(newContactInfo)
                              .ChangeAddress(newAddress);

        await _clients.UpdateAsync(updatedClient, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateClientResponse>.Ok(
            new UpdateClientResponse(true));
    }
}
