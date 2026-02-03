using Domain.Geography;

namespace Application.Services.Geography.DTOs;

public sealed record GetCitiesByCountyResponse(
    IReadOnlyList<CityDto> Cities);

public sealed record CityDto(
    int Id,
    string Name)
{
    public static CityDto From(City city)
    {
        return new CityDto(
            city.Id,
            city.Name);
    }
}
