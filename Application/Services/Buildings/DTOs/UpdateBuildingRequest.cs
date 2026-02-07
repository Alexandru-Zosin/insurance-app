using Application.Services.Shared.DTOs.BuildingDTOs;

namespace Application.Services.Buildings.DTOs;

public sealed record UpdateBuildingRequest(
    Guid BuildingId,
    BuildingCoreDto BuildingInfo);
