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
        Guid requestBuildingId, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(requestBuildingId, ct);
        if (building == null)
            return Result<GetBuildingDetailsResponse>.Fail(ErrorType.None, "Building was not found.");

        var policies = await _policies.GetByBuildingIdAsync(requestBuildingId, ct);

        var response = new GetBuildingDetailsResponse(
            BuildingDetailedDto.From(
                building, policies.Select(PolicyListItemDto.From).ToList()));

        return Result<GetBuildingDetailsResponse>.Ok(response);
    }

    public async Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(Guid requestClientId, CancellationToken ct = default)
    {
        var client = await _clients.GetByIdAsync(requestClientId, ct);

        if (client == null)
            return Result<GetBuildingsForClientResponse>.Fail(ErrorType.NotFound, "Client not found.");

        var buildings = await _buildings.GetByClientIdAsync(requestClientId, ct);
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

        var requestAddress = request.Building.Address.MapToDomain();
        var requestInsuredValue = request.Building.InsuredValue.MapToDomain();

        var building = Building.RegisterForClient(
            request.Building.OwnerClientId,
            requestAddress,
            request.Building.CityId,
            request.Building.ConstructionYear,
            request.Building.BuildingType,
            request.Building.SurfaceArea,
            requestInsuredValue,
            request.Building.ZoneRiskCategories);

        await _buildings.AddAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<RegisterBuildingResponse>.Ok(new RegisterBuildingResponse(building.Id));
    }

    public async Task<Result<UpdateBuildingResponse>> UpdateBuildingAsync(Guid requestBuildingId, UpdateBuildingRequest request, CancellationToken ct = default)
    {
        var building = await _buildings.GetByIdAsync(requestBuildingId, ct);
        if (building == null)
        {
            return Result<UpdateBuildingResponse>.Fail(
                ErrorType.NotFound,
                "Building not found");
        }

        var newSurfaceArea = request.BuildingInfo.SurfaceArea;
        var newInsuredValue = request.BuildingInfo.InsuredValue.MapToDomain();

        building.UpdateSurfaceArea(newSurfaceArea)
                .UpdateInsuredValue(newInsuredValue);

        await _buildings.UpdateAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}