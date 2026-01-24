using Application.Common;
using Domain.Common;
using Domain.Geography;

namespace Application.UseCases.Geography;

public sealed class GetCountriesService
    : IUseCase<
        GetCountriesService.Request,
        Result<GetCountriesService.Response>>
{
    public sealed record Request;

    public sealed record Response(
        IReadOnlyList<Country> Countries);

    private readonly ICountryRepository _countries;

    public GetCountriesService(ICountryRepository countries)
    {
        _countries = countries;
    }

    public async Task<Result<Response>> HandleAsync(
        Request _,
        CancellationToken ct = default)
    {
        var countries = await _countries.GetAllAsync(ct);

        return Result<Response>.Ok(
            new Response(countries));
    }
}
