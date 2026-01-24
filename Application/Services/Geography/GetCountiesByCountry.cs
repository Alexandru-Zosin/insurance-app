using Application.Common;
using Domain.Common;
using Domain.Geography;

namespace Application.UseCases.Geography;

public sealed class GetCountiesByCountryService
    : IUseCase<
        GetCountiesByCountryService.Request,
        Result<GetCountiesByCountryService.Response>>
{
    public sealed record Request(int CountryId);

    public sealed record Response(
        IReadOnlyList<County> Counties);

    private readonly ICountryRepository _countries;
    private readonly ICountyRepository _counties;

    public GetCountiesByCountryService(
        ICountryRepository countries,
        ICountyRepository counties)
    {
        _countries = countries;
        _counties = counties;
    }

    public async Task<Result<Response>> HandleAsync(
        Request request,
        CancellationToken ct = default)
    {
        var country = await _countries.GetByIdAsync(
            request.CountryId,
            ct);

        if (country == null)
        {
            return Result<Response>.Fail(
                ErrorType.NotFound,
                "Country not found");
        }

        var counties = await _counties.GetByCountryIdAsync(
            request.CountryId,
            ct);

        return Result<Response>.Ok(
            new Response(counties));
    }
}
