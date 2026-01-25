namespace Application.Services.Buildings.DTO;

public sealed record UpdateBuildingRequest(
    Guid BuildingId,
    int ConstructionYear,
    int SurfaceArea,
    decimal InsuredValue,
    string Currency,
    bool FloodRisk,
    bool EarthquakeRisk);
