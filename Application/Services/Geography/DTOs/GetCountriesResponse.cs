using Application.Services.Shared.DTOs.GeographyDTOs;

namespace Application.Services.Geography.DTOs;

public sealed record GetCountriesResponse(IReadOnlyList<CountryListItemDto> Countries);