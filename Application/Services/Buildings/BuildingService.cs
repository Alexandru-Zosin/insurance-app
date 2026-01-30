using Application.Common;
using Application.Repositories;
using Application.Services.Buildings.DTOs;
using Domain.Buildings;
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

        var address = Address.Create(request.Street, request.Number);
        var money = Money.Create(request.InsuredValue, request.Currency);
        
        var building = Building.Create(
            request.ClientId,
            address,
            city,
            request.ConstructionYear,
            Enum.Parse<BuildingType>(request.BuildingType),
            request.SurfaceArea,
            money,
            new RiskProfile(request.FloodRisk, request.EarthquakeRisk));

        await _buildings.AddAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<RegisterBuildingResponse>.Ok(
            new RegisterBuildingResponse
            {
                BuildingId = building.Id
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

        var money = Money.Create(request.InsuredValue, request.Currency);
        var riskProfile = new RiskProfile(request.FloodRisk, request.EarthquakeRisk); // Business logic needs updating
        building.UpdateConstructionYear(request.ConstructionYear)
                .UpdateSurfaceArea(request.SurfaceArea)
                .UpdateInsuredValue(money)
                .UpdateRiskProfile(riskProfile);

        await _buildings.UpdateAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}