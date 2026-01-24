using Application.Common;
using Domain.Buildings;
using Domain.Common;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Buildings;

public sealed class GetBuildingsForClientService
    : IUseCase<GetBuildingsForClientService.Request, Result<GetBuildingsForClientService.Response>>
{
    public sealed record Request(Guid ClientId);
    public sealed record Response(IReadOnlyList<Building> Buildings);

    private readonly IBuildingRepository _buildings;

    public GetBuildingsForClientService(IBuildingRepository buildings)
    {
        _buildings = buildings;
    }

    public async Task<Result<Response>> HandleAsync(Request request, CancellationToken ct = default)
    {
        var buildings = await _buildings.GetByClientIdAsync(request.ClientId, ct);
        return Result<Response>.Ok(
                    new Response(buildings));
    }
}
