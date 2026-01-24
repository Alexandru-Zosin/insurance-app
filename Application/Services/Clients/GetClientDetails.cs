using Application.Common;
using Domain.Buildings;
using Domain.Clients;
using Domain.Common;
using Domain.Policies;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class GetClientDetailsService
    : IUseCase<
        GetClientDetailsService.Request,
        Result<GetClientDetailsService.Response>>
{
    public sealed record Request(Guid ClientId);

    public sealed record Response(
        Client Client,
        IReadOnlyList<Building> Buildings,
        IReadOnlyList<Policy> Policies);

    private readonly IClientRepository _clients;
    private readonly IBuildingRepository _buildings;
    private readonly IPolicyRepository _policies;

    public GetClientDetailsService(
        IClientRepository clients,
        IBuildingRepository buildings,
        IPolicyRepository policies)
    {
        _clients = clients;
        _buildings = buildings;
        _policies = policies;
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

        var buildings = await _buildings.GetByClientIdAsync(request.ClientId, ct);
        var policies = await _policies.GetByClientIdAsync(request.ClientId, ct);

        return Result<Response>.Ok(
            new Response(client, buildings, policies));
    }
}
