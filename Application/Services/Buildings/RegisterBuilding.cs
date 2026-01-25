using Application.Common;
using Application.Services.Buildings.DTO;
using Domain.Buildings;
using Domain.Common;
using Domain.Geography;
using Domain.Shared;
using Infrastructure.Persistence.Repositories;

namespace Application.Services.Buildings;

public sealed class RegisterBuildingService
    : IUseCase<
        RegisterBuildingRequest,
        Result<RegisterBuildingResponse>>
{
    private readonly IBuildingRepository _buildings;
    private readonly ICityRepository _cities;

    public RegisterBuildingService(
        IBuildingRepository buildings,
        ICityRepository cities)
    {
        _buildings = buildings;
        _cities = cities;
    }

    public async Task<Result<RegisterBuildingResponse>> HandleAsync(
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

        return Result<RegisterBuildingResponse>.Ok(
            new RegisterBuildingResponse{
                BuildingId = buildingResult.Value!.Id
            });
    }
}
