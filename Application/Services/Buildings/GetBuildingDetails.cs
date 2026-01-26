using Application.Common;
using Domain.Common;
using Application.Services.Buildings.DTO;
using Domain.Policies;
using Infrastructure.Persistence.Repositories;

namespace Application.Services.Buildings;

public sealed class GetBuildingDetailsService
    : IUseCase<GetBuildingDetailsRequest, Result<GetBuildingDetailsResponse>>
{
    private readonly IBuildingRepository _buildings;
    private readonly IPolicyRepository _policies;

    public GetBuildingDetailsService(
        IBuildingRepository buildings,
        IPolicyRepository policies)
    {
        _buildings = buildings;
        _policies = policies;
    }

    public async Task<Result<GetBuildingDetailsResponse>> HandleAsync(
        GetBuildingDetailsRequest request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(request.BuildingId, ct);

        if (building == null)
        {
            return Result<GetBuildingDetailsResponse>.Fail(ErrorType.NotFound, "Building not found");
        }

        var policies = await _policies.GetByBuildingIdAsync(request.BuildingId, ct);

        var response = new GetBuildingDetailsResponse
        {
            Building = BuildingDto.From(building),
            Policies = policies.Select(PolicyDto.From).ToList()
        };

        return Result<GetBuildingDetailsResponse>.Ok(response);
    }
}
