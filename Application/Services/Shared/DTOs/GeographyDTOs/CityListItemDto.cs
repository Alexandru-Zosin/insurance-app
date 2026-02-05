using Domain.Geography;

namespace Application.Services.Shared.DTOs.GeographyDTOs;

    public sealed record CityListItemDto(int Id, string Name, int CountyId)
{
    public static CityListItemDto From(City c) =>
       new(c.Id, c.Name, c.CountyId);
}
