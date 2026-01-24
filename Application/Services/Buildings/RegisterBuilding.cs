using Application.Common;
using Domain.Buildings;
using Domain.Common;
using Domain.Geography;
using Domain.Shared;
using Infrastructure.Persistence.Repositories;

namespace Application.UseCases.Buildings;

public sealed class RegisterBuildingService
    : IUseCase<
        RegisterBuildingService.Request,
        Result<RegisterBuildingService.Response>>
{
    public sealed record Request(
        Guid ClientId,
        int CityId,
        string Street,
        string Number,
        int ConstructionYear,
        string BuildingType,
        int SurfaceArea,
        decimal InsuredValue,
        string Currency,
        bool FloodRisk,
        bool EarthquakeRisk);

    public sealed record Response(Guid BuildingId);

    private readonly IBuildingRepository _buildings;
    private readonly ICityRepository _cities;

    public RegisterBuildingService(
        IBuildingRepository buildings,
        ICityRepository cities)
    {
        _buildings = buildings;
        _cities = cities;
    }

    public async Task<Result<Response>> HandleAsync(
        Request request,
        CancellationToken ct = default)
    {
        var city = await _cities.GetByIdAsync(request.CityId, ct);
        if (city == null)
        {
            return Result<Response>.Fail(
                ErrorType.NotFound,
                "City not found");
        }

        var addressResult = Address.Create(request.Street, request.Number);
        if (!addressResult.IsSuccess)
        {
            return Result<Response>.Fail(
                addressResult.ErrorType,
                addressResult.ErrorMessage);
        }

        var moneyResult = Money.Create(request.InsuredValue, request.Currency);
        if (!moneyResult.IsSuccess)
        {
            return Result<Response>.Fail(
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
            return Result<Response>.Fail(
                buildingResult.ErrorType,
                buildingResult.ErrorMessage);
        }

        await _buildings.AddAsync(buildingResult.Value!, ct);

        return Result<Response>.Ok(
            new Response(buildingResult.Value!.Id));
    }
}
