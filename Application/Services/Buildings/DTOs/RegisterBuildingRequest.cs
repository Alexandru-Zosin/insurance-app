using Application.Services.Shared.DTOs.BuildingDTOs;

namespace Application.Services.Buildings.DTOs;

public sealed record RegisterBuildingRequest(BuildingCoreDto Building);