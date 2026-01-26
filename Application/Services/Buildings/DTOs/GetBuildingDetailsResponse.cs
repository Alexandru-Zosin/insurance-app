using Domain.Buildings;
using Domain.Policies;

namespace Application.Services.Buildings.DTO;

public sealed class GetBuildingDetailsResponse
{
    public BuildingDto Building { get; init; } = null!;
    public IReadOnlyList<PolicyDto> Policies { get; init; } = [];
}

public sealed class BuildingDto
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }

    public string Street { get; init; } = null!;
    public string Number { get; init; } = null!;

    public int CityId { get; init; }
    public string CityName { get; init; } = null!;

    public int ConstructionYear { get; init; }
    public string BuildingType { get; init; } = null!;
    public int SurfaceArea { get; init; }

    public decimal InsuredValue { get; init; }
    public string Currency { get; init; } = null!;

    public bool FloodRisk { get; init; }
    public bool EarthquakeRisk { get; init; }

    public static BuildingDto From(Building building)
    {
        return new BuildingDto
        {
            Id = building.Id,
            ClientId = building.ClientId,
            Street = building.Address.Street,
            Number = building.Address.Number,
            CityId = building.City.Id,
            CityName = building.City.Name,
            ConstructionYear = building.ConstructionYear,
            BuildingType = building.BuildingType.ToString(),
            SurfaceArea = building.SurfaceArea,
            InsuredValue = building.InsuredValue.Amount,
            Currency = building.InsuredValue.Currency,
            FloodRisk = building.RiskProfile.FloodRisk,
            EarthquakeRisk = building.RiskProfile.EarthquakeRisk
        };
    }
}

public sealed class PolicyDto
{
    public Guid Id { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public decimal Premium { get; init; }
    public string Currency { get; init; } = null!;

    public static PolicyDto From(Policy p)
    {
        return new PolicyDto
        {
            Id = p.Id,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Premium = p.Premium.Amount,
            Currency = p.Premium.Currency
        };
    }
}
