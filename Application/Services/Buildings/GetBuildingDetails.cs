using Application.Common;
using Domain.Buildings;
using Domain.Common;
using Domain.Policies;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Buildings;

public sealed class GetBuildingDetailsService
    : IUseCase<GetBuildingDetailsService.Request, Result<GetBuildingDetailsService.Response>>
{
    public sealed record Request(Guid BuildingId);

    public sealed record Response(
        Building Building,
        IReadOnlyList<Policy> Policies);

    private readonly IBuildingRepository _buildings;
    private readonly IPolicyRepository _policies;

    public GetBuildingDetailsService(
        IBuildingRepository buildings,
        IPolicyRepository policies)
    {
        _buildings = buildings;
        _policies = policies;
    }

    public async Task<Result<Response>> HandleAsync(Request request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(request.BuildingId, ct);

        if (building == null)
        {
            return Result<Response>.Fail(ErrorType.NotFound, "Building not found");
        }


        var policies = await _policies.GetByBuildingIdAsync(request.BuildingId, ct);
        return Result<Response>.Ok(new Response(building, policies));
    }
}
