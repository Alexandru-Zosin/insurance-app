using Application.Common;
using Application.Services.Geography.DTO;
using Domain.Common;
using Domain.Geography;

namespace Application.UseCases.Geography;

public sealed class GetCitiesByCountyService
    : IUseCase<
        GetCitiesByCountyRequest,
        Result<GetCitiesByCountyResponse>>
{
    private readonly ICityRepository _cities;

    public GetCitiesByCountyService(ICityRepository cities)
    {
        _cities = cities;
    }

    public async Task<Result<GetCitiesByCountyResponse>> HandleAsync(
        GetCitiesByCountyRequest request,
        CancellationToken ct = default)
    {
        var cities = await _cities.GetByCountyIdAsync(
            request.CountyId,
            ct);

        return Result<GetCitiesByCountyResponse>.Ok(
           new GetCitiesByCountyResponse(
               cities.Select(CityDto.From).ToList()));
    }
}
