using Application.Common;
using Application.Repositories;
using Application.Repositories.SearchCriteria;
using Application.Services.Buildings.DTOs;
using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Buildings;

namespace Application.Services.Buildings;

public sealed class BuildingService(
    IBuildingRepository buildingRepository,
    IPolicyRepository policyRepository,
    IClientRepository clientRepository,
    ICityRepository cityRepository,
    IUnitOfWork uow) : IBuildingService

{
    public async Task<Result<GetBuildingDetailsResponse>> GetBuildingDetailsAsync(
        Guid buildingId, CancellationToken ct = default)
    {
        var building = await buildingRepository.GetByIdAsync(buildingId, ct);
        if (building == null)
            return Result<GetBuildingDetailsResponse>.Fail(ErrorType.None, "Building was not found.");

        var buildingPolicies = await policyRepository.ListAsync(
            PolicySearchCriteria.ByBuildingId(buildingId), ct: ct);

        var response = new GetBuildingDetailsResponse(
            BuildingDetailedDto.From(
                building, buildingPolicies.Select(PolicyListItemDto.From).ToList()));

        return Result<GetBuildingDetailsResponse>.Ok(response);
    }

    public async Task<Result<GetBuildingsForClientResponse>> GetBuildingsForClientAsync(Guid clientId, CancellationToken ct = default)
    {
        var client = await clientRepository.GetByIdAsync(clientId, ct);

        if (client == null)
            return Result<GetBuildingsForClientResponse>.Fail(ErrorType.NotFound, "Client not found.");

        var clientBuildings = await buildingRepository.ListByClientIdAsync(clientId, ct);
        var response = new GetBuildingsForClientResponse(clientBuildings.Select(BuildingListItemDto.From).ToList());

        return Result<GetBuildingsForClientResponse>.Ok(response);
    }

    public async Task<Result<RegisterBuildingResponse>> RegisterBuildingAsync(
        RegisterBuildingRequest request,
        CancellationToken ct = default)
    {
        var existingCity = await cityRepository.GetByIdAsync(request.Building.CityId, ct);
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

        await buildingRepository.AddAsync(newBuilding, ct);
        await uow.SaveChangesAsync(ct);

        return Result<RegisterBuildingResponse>.Ok(new RegisterBuildingResponse(newBuilding.Id));
    }

    public async Task<Result<UpdateBuildingResponse>> UpdateBuildingAsync(Guid buildingId, UpdateBuildingRequest request, CancellationToken ct = default)
    {
        var building = await buildingRepository.GetByIdAsync(buildingId, ct);
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

        await buildingRepository.UpdateAsync(building, ct);
        await uow.SaveChangesAsync(ct);

        return Result<UpdateBuildingResponse>.Ok(
            new UpdateBuildingResponse(true));
    }
}