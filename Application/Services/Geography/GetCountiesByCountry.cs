using Application.Common;
using Application.Services.Geography.DTO;
using Domain.Common;
using Domain.Geography;

namespace Application.UseCases.Geography;

public sealed class GetCountiesByCountryService
    : IUseCase<
        GetCountiesByCountryRequest,
        Result<GetCountiesByCountryResponse>>
{
    private readonly ICountryRepository _countries;
    private readonly ICountyRepository _counties;

    public GetCountiesByCountryService(
        ICountryRepository countries,
        ICountyRepository counties)
    {
        _countries = countries;
        _counties = counties;
    }

    public async Task<Result<GetCountiesByCountryResponse>> HandleAsync(
        GetCountiesByCountryRequest request,
        CancellationToken ct = default)
    {
        var country = await _countries.GetByIdAsync(
            request.CountryId,
            ct);

        if (country == null)
        {
            return Result<GetCountiesByCountryResponse>.Fail(
                ErrorType.NotFound,
                "Country not found");
        }

        var counties = await _counties.GetByCountryIdAsync(
            request.CountryId,
            ct);

        return Result<GetCountiesByCountryResponse>.Ok(
            new GetCountiesByCountryResponse(
                counties.Select(CountyDto.From).ToList()));
    }
}
