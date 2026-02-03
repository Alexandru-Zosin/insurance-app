namespace Application.Services.Buildings.DTOs;

public sealed record UpdateBuildingRequest(
    Guid BuildingId,
    int ConstructionYear,
    int SurfaceArea,
    decimal InsuredValue,
    string Currency,
    bool FloodRisk,
    bool EarthquakeRisk);
