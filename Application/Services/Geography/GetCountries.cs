using Application.Common;
using Domain.Common;
using Domain.Geography;
using Application.Services.Geography.DTO;

namespace Application.UseCases.Geography;

public sealed class GetCountries
    : IUseCase<
        GetCountriesRequest,
        Result<GetCountriesResponse>>
{
    private readonly ICountryRepository _countries;

    public GetCountries(ICountryRepository countries)
    {
        _countries = countries;
    }

    public async Task<Result<GetCountriesResponse>> HandleAsync(
        GetCountriesRequest _,
        CancellationToken ct = default)
    {
        var countries = await _countries.GetAllAsync(ct);

        return Result<GetCountriesResponse>.Ok(
           new GetCountriesResponse(
               countries.Select(CountryDto.From).ToList()));
    }
}
