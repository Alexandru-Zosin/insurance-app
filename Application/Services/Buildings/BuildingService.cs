using Application.Common;
using Application.Repositories;
using Application.Services.Buildings.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Buildings;

namespace Application.Services.Buildings;

public sealed class BuildingService(
    IBuildingRepository _buildingRepository,
    IPolicyRepository _policyRepository,
    IClientRepository _clientRepository,
    ICityRepository _cityRepository,
    IUnitOfWork _uow) : IBuildingService

{
    public async Task<Result<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(
        Guid buildingId, CancellationToken ct = default)
    {
        var building = await _buildingRepository.GetByIdAsync(buildingId, ct);
        if (building == null)
            return Result<GetBuildingDetailsResponse>.Fail(ErrorType.None, "Building was not found.");

        var buildingPolicies = await _policyRepository.GetByBuildingIdAsync(buildingId, ct);

        var response = new GetBuildingDetailsResponse(
            BuildingDetailedDto.From(
                building, buildingPolicies.Select(PolicyListItemDto.From).ToList()));

        return Result<GetBuildingDetailsResponse>.Ok(response);
    }

    public async Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await _clientRepository.GetByIdAsync(clientId, ct);

        if (client == null)
            return Result<GetBuildingsForClientResponse>.Fail(ErrorType.NotFound, "Client not found.");

        var clientBuildings = await _buildingRepository.GetByClientIdAsync(clientId, ct);
        var response = new GetBuildingsForClientResponse(clientBuildings.Select(BuildingListItemDto.From).ToList());

        return Result<GetBuildingsForClientResponse>.Ok(response);
    }

    public async Task<Result<RegisterBuildingResponse>> RegisterBuildingAsync(
        RegisterBuildingRequest request,
        CancellationToken ct = default)
    {
        var existingCity = await _cityRepository.GetByIdAsync(request.Building.CityId, ct);
        if (existingCity == null)
        {
            return Result<RegisterBuildingResponse>.Fail(
                ErrorType.NotFound,
                "City not found");
        }

        var newBuilding = Building.RegisterForClient(
            request.Building.OwnerClientId,
            request.Building.Address.MapToDomain(),
            request.Building.CityId,
            request.Building.ConstructionYear,
            request.Building.BuildingType,
            request.Building.SurfaceArea,
            request.Building.InsuredValue.MapToDomain(),
            request.Building.ZoneRiskCategories);

        await _buildingRepository.AddAsync(newBuilding, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<RegisterBuildingResponse>.Ok(new RegisterBuildingResponse(newBuilding.Id));
    }

    public async Task<Result<UpdateBuildingResponse>> UpdateBuildingAsync(Guid buildingId, UpdateBuildingRequest request, CancellationToken ct = default)
    {
        var building = await _buildingRepository.GetByIdAsync(buildingId, ct);
        if (building == null)
        {
            return Result<UpdateBuildingResponse>.Fail(
                ErrorType.NotFound,
                "Building not found");
        }

        var updatedSurfaceArea = request.BuildingInfo.SurfaceArea;
        var updatedInsuredValue = request.BuildingInfo.InsuredValue.MapToDomain();

        building.UpdateSurfaceArea(updatedSurfaceArea)
                .UpdateInsuredValue(updatedInsuredValue);

        await _buildingRepository.UpdateAsync(building, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}