using Application.Common;
using Application.Repositories;
using Application.Services.Buildings.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Buildings;

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

        var response = new GetBuildingDetailsResponse(
            BuildingDetailedDto.From(
                building, policies.Select(PolicyListItemDto.From).ToList()));

        return Result<GetBuildingDetailsResponse>.Ok(response);
    }

    public async Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(
        GetBuildingsForClientRequest request, CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(request.ClientId, ct);

        if (client == null)
            return Result<GetBuildingsForClientResponse>.Fail(ErrorType.NotFound, "Client not found.");

        var buildings = await _buildings.GetByClientIdAsync(request.ClientId, ct);
        var response = new GetBuildingsForClientResponse(
            buildings.Select(BuildingListItemDto.From).ToList());

        return Result<GetBuildingsForClientResponse>.Ok(response);
    }

    public async Task<Result<RegisterBuildingResponse>> RegisterBuildingAsync(
        RegisterBuildingRequest request,
        CancellationToken ct = default)
    {
        var city = await _cities.GetByIdAsync(request.Building.CityId, ct);
        if (city == null)
        {
            return Result<RegisterBuildingResponse>.Fail(
                ErrorType.NotFound,
                "City not found");
        }

        var address = request.Building.Address.ToDomain();
        var money = request.Building.InsuredValue.ToDomain();

        var building = Building.RegisterForClient(
            request.Building.OwnerClientId,
            address,
            city.Id,
            request.Building.ConstructionYear,
            request.Building.BuildingType,
            request.Building.SurfaceArea,
            money,
            request.Building.ZoneRiskCategories);

        await _buildings.AddAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<RegisterBuildingResponse>.Ok(new RegisterBuildingResponse(building.Id));
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

        var newSurfaceArea = request.BuildingInfo.SurfaceArea;
        var newInsuredValue = request.BuildingInfo.InsuredValue.ToDomain();

        building.UpdateSurfaceArea(newSurfaceArea)
                .UpdateInsuredValue(newInsuredValue);

        await _buildings.UpdateAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}