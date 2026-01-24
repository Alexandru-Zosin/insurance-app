using Application.Common;
using Domain.Shared;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Buildings;

public sealed class UpdateBuildingService
    : IUseCase<UpdateBuildingService.Request, UpdateBuildingService.Response>
{
    public sealed record Request(
        Guid BuildingId,
        int ConstructionYear,
        int SurfaceArea,
        decimal InsuredValue,
        string Currency,
        bool FloodRisk,
        bool EarthquakeRisk);

    public sealed record Response(bool Updated);

    private readonly IBuildingRepository _buildings;

    public UpdateBuildingService(IBuildingRepository buildings)
    {
        _buildings = buildings;
    }

    public async Task<Response> HandleAsync(Request request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(request.BuildingId, ct);
        if (building == null)
            return new Response(false);

        building.UpdateConstructionYear(request.ConstructionYear);
        building.UpdateSurfaceArea(request.SurfaceArea);
        building.UpdateInsuredValue(
            Money.Create(request.InsuredValue, request.Currency).Value!);
        building.UpdateRiskProfile(
            new RiskProfile(request.FloodRisk, request.EarthquakeRisk));

        await _buildings.UpdateAsync(building, ct);
        return new Response(true);
    }
}
