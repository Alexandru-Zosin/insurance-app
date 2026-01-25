using Application.Common;
using Domain.Common;
using Domain.Shared;
using Infrastructure.Persistence.Repositories;
using Application.Services.Buildings.DTO;

namespace Application.Services.Buildings.UpdateBuilding;

public sealed class UpdateBuildingService
    : IUseCase<UpdateBuildingRequest, Result<UpdateBuildingResponse>>
{
    private readonly IBuildingRepository _buildings;

    public UpdateBuildingService(IBuildingRepository buildings) 
    {
        _buildings = buildings;
    }

    public async Task<Result<UpdateBuildingResponse>> HandleAsync(
        UpdateBuildingRequest request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(request.BuildingId, ct);
        if (building == null)
        {
            return Result<UpdateBuildingResponse>.Fail(
                ErrorType.NotFound,
                "Building not found");
        }

        var yearResult = building.UpdateConstructionYear(request.ConstructionYear);
        if (!yearResult.IsSuccess)
            return Result<UpdateBuildingResponse>.Fail(yearResult.ErrorType, yearResult.ErrorMessage);

        var areaResult = building.UpdateSurfaceArea(request.SurfaceArea);
        if (!areaResult.IsSuccess)
            return Result<UpdateBuildingResponse>.Fail(areaResult.ErrorType, areaResult.ErrorMessage);

        var moneyResult = Money.Create(request.InsuredValue, request.Currency);
        if (!moneyResult.IsSuccess)
            return Result<UpdateBuildingResponse>.Fail(moneyResult.ErrorType, moneyResult.ErrorMessage);

        var valueResult = building.UpdateInsuredValue(moneyResult.Value!);
        if (!valueResult.IsSuccess)
            return Result<UpdateBuildingResponse>.Fail(valueResult.ErrorType, valueResult.ErrorMessage);

        var riskResult = building.UpdateRiskProfile(
            new RiskProfile(request.FloodRisk, request.EarthquakeRisk));
        if (!riskResult.IsSuccess)
            return Result<UpdateBuildingResponse>.Fail(riskResult.ErrorType, riskResult.ErrorMessage);

        await _buildings.UpdateAsync(building, ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}