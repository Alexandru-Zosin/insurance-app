using Application.Services.Geography.DTOs;
using Domain.Common;

namespace Application.Services.Geography
{
    public interface IGeographyService
    {
        Task<Result<GetCitiesByCountyResponse>> GetCitiesByCountyAsync(GetCitiesByCountyRequest request, CancellationToken ct = default);
        Task<Result<GetCountiesByCountryResponse>> GetCountiesByCountryAsync(GetCountiesByCountryRequest request, CancellationToken ct = default);
        Task<Result<GetCountriesResponse>> GetCountriesAsync(GetCountriesRequest _, CancellationToken ct = default);
    }
}