using Domain.Geography;

namespace Application.Services.Geography.DTO;

public sealed record GetCountiesByCountryResponse(
    IReadOnlyList<CountyDto> Counties);

public sealed record CountyDto(
    int Id,
    string Name)
{
    public static CountyDto From(County county)
    {
        return new CountyDto(
            county.Id,
            county.Name);
    }
}
