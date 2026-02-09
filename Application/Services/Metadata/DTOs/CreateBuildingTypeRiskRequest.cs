using Domain.Buildings;

namespace Application.Services.Metadata.DTOs;

public sealed record CreateBuildingTypeRiskRequest(
    string Name,
    decimal Percentage,
    bool IsActive,
    BuildingType BuildingType
);
