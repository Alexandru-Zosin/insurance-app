using Domain.Geography;

namespace Application.Services.Shared.DTOs.GeographyDTOs;

public sealed record CountryListItemDto(int Id, string Name)
{
    public static CountryListItemDto From(Country c) => new(c.Id, c.Name);
}