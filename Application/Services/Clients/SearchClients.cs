using Application.Common;
using Domain.Clients;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class SearchClientsService
    : IUseCase<SearchClientsService.Request, SearchClientsService.Response>
{
    public sealed record Request(string? Name, string? Identifier);
    public sealed record Response(IReadOnlyList<Client> Clients);

    private readonly IClientRepository _clients;

    public SearchClientsService(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<Response> HandleAsync(Request request, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(request.Identifier))
        {
            var client = await _clients.GetByRegistrationNumberAsync(request.Identifier, ct);
            return new Response(
                client == null ? Array.Empty<Client>() : new[] { client });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var results = await _clients.SearchByNameAsync(request.Name, ct);
            return new Response(results);
        }

        return new Response(Array.Empty<Client>());
    }
}
