using Domain.Geography;
namespace Application.Repositories;
public interface ICountryRepository
{
    void Add(Country country);
    Task<Country?> GetByIdAsync(int countryId, CancellationToken ct = default);
    Task<IReadOnlyList<Country>> ListAsync(CancellationToken ct = default);
}
