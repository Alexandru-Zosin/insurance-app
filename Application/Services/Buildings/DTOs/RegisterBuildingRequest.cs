namespace Application.Services.Buildings.DTO;

public sealed record RegisterBuildingRequest
{
    public Guid ClientId { get; init; }

    public int CityId { get; init; }

    public string Street { get; init; } = null!;
    public string Number { get; init; } = null!;

    public int ConstructionYear { get; init; }

    public string BuildingType { get; init; } = null!;

    public int SurfaceArea { get; init; }

    public decimal InsuredValue { get; init; }
    public string Currency { get; init; } = null!;

    public bool FloodRisk { get; init; }
    public bool EarthquakeRisk { get; init; }
}
