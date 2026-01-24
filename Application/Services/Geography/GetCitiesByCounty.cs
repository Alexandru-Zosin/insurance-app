using Application.Common;
using Domain.Common;
using Domain.Geography;

namespace Application.UseCases.Geography;

public sealed class GetCitiesByCountyService
    : IUseCase<
        GetCitiesByCountyService.Request,
        Result<GetCitiesByCountyService.Response>>
{
    public sealed record Request(int CountyId);

    public sealed record Response(
        IReadOnlyList<City> Cities);

    private readonly ICityRepository _cities;

    public GetCitiesByCountyService(ICityRepository cities)
    {
        _cities = cities;
    }

    public async Task<Result<Response>> HandleAsync(
        Request request,
        CancellationToken ct = default)
    {
        var cities = await _cities.GetByCountyIdAsync(
            request.CountyId,
            ct);

        return Result<Response>.Ok(
            new Response(cities));
    }
}
