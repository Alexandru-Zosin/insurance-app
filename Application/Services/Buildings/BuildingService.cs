using Application.Common;
using Application.Services.Buildings.DTOs;
using Domain.Buildings;
using Domain.Clients;
using Domain.Common;
using Domain.Geography;
using Domain.Policies;
using Domain.Shared;

namespace Application.Services.Buildings;

public sealed class BuildingService(
    IBuildingRepository _buildings,
    IPolicyRepository _policies,
    IClientRepository _clients,
    ICityRepository _cities,
    IUnitOfWork _uow) : IBuildingService

{
    public async Task<Result<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(
        GetBuildingDetailsRequest request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(request.BuildingId, ct);
        if (building == null)
            return Result<GetBuildingDetailsResponse>.Fail(ErrorType.None, "Building was not found.");

        var policies = await _policies.GetByBuildingIdAsync(request.BuildingId, ct);

        var response = new GetBuildingDetailsResponse
        {
            Building = BuildingDto.From(building),
            Policies = policies.Select(PolicyDto.From).ToList()
        };

        return Result<GetBuildingDetailsResponse>.Ok(response);
    }

    public async Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(
        GetBuildingsForClientRequest request, CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.ClientId, ct);

        if (client == null)
            return Result<GetBuildingsForClientResponse>.Fail(ErrorType.NotFound, "Client not found.");

        var buildings = await _buildings.GetByClientIdAsync(request.ClientId, ct);
        var response = new GetBuildingsForClientResponse
        {
            Buildings = buildings.Select(BuildingSummaryDto.From).ToList()
        };

        return Result<GetBuildingsForClientResponse>.Ok(response);
    }

    public async Task<Result<RegisterBuildingResponse>> RegisterBuildingAsync(
        RegisterBuildingRequest request,
        CancellationToken ct = default)
    {
        var city = await _cities.GetByIdAsync(request.CityId, ct);
        if (city == null)
        {
            return Result<RegisterBuildingResponse>.Fail(
                ErrorType.NotFound,
                "City not found");
        }

        var addressResult = Address.Create(request.Street, request.Number);
        if (!addressResult.IsSuccess)
        {
            return Result<RegisterBuildingResponse>.Fail(
                addressResult.ErrorType,
                addressResult.ErrorMessage);
        }

        var moneyResult = Money.Create(request.InsuredValue, request.Currency);
        if (!moneyResult.IsSuccess)
        {
            return Result<RegisterBuildingResponse>.Fail(
                moneyResult.ErrorType,
                moneyResult.ErrorMessage);
        }

        var buildingResult = Building.Create(
            request.ClientId,
            addressResult.Value!,
            city,
            request.ConstructionYear,
            Enum.Parse<BuildingType>(request.BuildingType),
            request.SurfaceArea,
            moneyResult.Value!,
            new RiskProfile(request.FloodRisk, request.EarthquakeRisk));

        if (!buildingResult.IsSuccess)
        {
            return Result<RegisterBuildingResponse>.Fail(
                buildingResult.ErrorType,
                buildingResult.ErrorMessage);
        }

        await _buildings.AddAsync(buildingResult.Value!, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<RegisterBuildingResponse>.Ok(
            new RegisterBuildingResponse
            {
                BuildingId = buildingResult.Value!.Id
            });
    }

    public async Task<Result<UpdateBuildingResponse>> UpdateBuildingAsync(
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
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}