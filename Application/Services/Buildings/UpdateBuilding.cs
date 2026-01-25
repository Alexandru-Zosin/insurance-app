using Application.Common;
using Domain.Common;
using Domain.Shared;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Buildings;

public sealed class UpdateBuildingService
    : IUseCase<UpdateBuildingService.Request, Result<UpdateBuildingService.Response>>
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

    public async Task<Result<Response>> HandleAsync(Request request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(request.BuildingId, ct);
        if (building == null)
        {
            return Result<Response>.Fail(
                ErrorType.NotFound,
                "Building not found");
        }

        var yearResult = building.UpdateConstructionYear(request.ConstructionYear);
        if (!yearResult.IsSuccess)
            return Result<Response>.Fail(yearResult.ErrorType, yearResult.ErrorMessage);

        var areaResult = building.UpdateSurfaceArea(request.SurfaceArea);
        if (!areaResult.IsSuccess)
            return Result<Response>.Fail(areaResult.ErrorType, areaResult.ErrorMessage);

        var moneyResult = Money.Create(request.InsuredValue, request.Currency);
        if (!moneyResult.IsSuccess)
            return Result<Response>.Fail(moneyResult.ErrorType, moneyResult.ErrorMessage);

        var valueResult = building.UpdateInsuredValue(moneyResult.Value!);
        if (!valueResult.IsSuccess)
            return Result<Response>.Fail(valueResult.ErrorType, valueResult.ErrorMessage);

        var riskResult = building.UpdateRiskProfile(
            new RiskProfile(request.FloodRisk, request.EarthquakeRisk));
        if (!riskResult.IsSuccess)
            return Result<Response>.Fail(riskResult.ErrorType, riskResult.ErrorMessage);

        await _buildings.UpdateAsync(building, ct);

        return Result<Response>.Ok(
            new Response(true));
    }
}