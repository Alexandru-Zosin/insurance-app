using Application.Common;
using Application.Services.Buildings.DTO;
using Domain.Common;
using Infrastructure.Persistence.Repositories;

namespace Application.Services.Buildings.GetBuildingsForClient;

public sealed class GetBuildingsForClientService
    : IUseCase<GetBuildingsForClientRequest, Result<GetBuildingsForClientResponse>>
{

    private readonly IBuildingRepository _buildings;

    public GetBuildingsForClientService(IBuildingRepository buildings)
    {
        _buildings = buildings;
    }

    public async Task<Result<GetBuildingsForClientResponse>> HandleAsync(
        GetBuildingsForClientRequest request, CancellationToken ct = default)
    {
        var buildings = await _buildings.GetByClientIdAsync(request.ClientId, ct);
        var response = new GetBuildingsForClientResponse
        {
            Buildings = buildings.Select(BuildingSummaryDto.From).ToList()
        };

        return Result<GetBuildingsForClientResponse>.Ok(response);
    }
}
