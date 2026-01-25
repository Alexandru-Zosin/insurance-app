using Domain.Buildings;

namespace Application.Services.Buildings.DTO;

public sealed class GetBuildingsForClientResponse
{
    public IReadOnlyList<BuildingSummaryDto> Buildings { get; init; } = [];
}

public sealed class BuildingSummaryDto
{
    public Guid Id { get; init; }

    public int CityId { get; init; }
    public string CityName { get; init; } = null!;

    public string BuildingType { get; init; } = null!;
    public int ConstructionYear { get; init; }
    public int SurfaceArea { get; init; }

    public decimal InsuredValue { get; init; }
    public string Currency { get; init; } = null!;

    public static BuildingSummaryDto From(Building building)
    {
        return new BuildingSummaryDto
        {
            Id = building.Id,
            CityId = building.City.Id,
            CityName = building.City.Name,
            BuildingType = building.BuildingType.ToString(),
            ConstructionYear = building.ConstructionYear,
            SurfaceArea = building.SurfaceArea,
            InsuredValue = building.InsuredValue.Amount,
            Currency = building.InsuredValue.Currency
        };
    }
}
