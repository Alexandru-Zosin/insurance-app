namespace Application.Services.Buildings.DTOs;

public sealed record RegisterBuildingRequest(
    Guid ClientId,
    int CityId,
    string Street,
    string Number,
    int ConstructionYear,
    string BuildingType,
    int SurfaceArea,
    decimal InsuredValue,
    string Currency,
    bool FloodRisk,
    bool EarthquakeRisk);