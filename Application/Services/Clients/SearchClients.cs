using Application.Common;
using Application.Services.Clients.DTO;
using Domain.Clients;
using Domain.Common;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class SearchClientsService
    : IUseCase<SearchClientsRequest, Result<SearchClientsResponse>>
{
    private readonly IClientRepository _clients;

    public SearchClientsService(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<Result<SearchClientsResponse>> HandleAsync(
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
}
