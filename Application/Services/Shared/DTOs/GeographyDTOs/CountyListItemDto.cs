
using Domain.Geography;

namespace Application.Services.Shared.DTOs.GeographyDTOs;

public sealed record CountyListItemDto(int Id, string Name, int CountryId)
{
    public static CountyListItemDto From(County c) => new(c.Id, c.Name, c.CountryId);
}
