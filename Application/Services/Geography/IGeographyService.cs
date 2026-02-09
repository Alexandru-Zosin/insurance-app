using Application.Services.Geography.DTOs;
using Application.Common;

namespace Application.Services.Geography
{
    public interface IGeographyService
    {
        Task<Result<GetCitiesByCountyResponse>> GetCitiesByCountyAsync(int countyId, CancellationToken ct = default);
        Task<Result<GetCountiesByCountryResponse>> GetCountiesByCountryAsync(int countryId, CancellationToken ct = default);
        Task<Result<GetCountriesResponse>> GetCountriesAsync(CancellationToken ct = default);
    }
}