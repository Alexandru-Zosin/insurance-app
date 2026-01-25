using Application.Common;
using Application.Services.Clients.DTO;
using Domain.Common;
using Domain.Policies;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Clients;

public sealed class GetClientDetailsService
    : IUseCase<
        GetClientDetailsRequest,
        Result<GetClientDetailsResponse>>
{
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

    public async Task<Result<GetClientDetailsResponse>> HandleAsync(
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
}
