using Application.Services.Clients.DTOs;
using Application.Common;

namespace Application.Services.Clients;

public interface IClientService
{
    Task<Result<CreateClientResponse>> CreateClientAsync(CreateClientRequest request, CancellationToken ct = default);
    Task<Result<GetClientDetailsResponse>> GetClientDetailsAsync(Guid clientId, CancellationToken ct = default);
    Task<Result<SearchClientsResponse>> SearchClientsAsync(SearchClientsRequest request, CancellationToken ct = default);
    Task<Result<UpdateClientResponse>> UpdateClientAsync(Guid clientId, UpdateClientRequest request, CancellationToken ct = default);
}