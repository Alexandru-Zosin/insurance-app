using Domain.Geography;

namespace Application.Services.Geography.DTO;

public sealed record GetCountriesResponse(
    IReadOnlyList<CountryDto> Countries);

public sealed record CountryDto(
    int Id,
    string Name)
{
    public static CountryDto From(Country country)
    {
        return new CountryDto(
            country.Id,
            country.Name);
    }
}
