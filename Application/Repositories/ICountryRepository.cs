using Domain.Geography;
namespace Application.Repositories;
public interface ICountryRepository
{
    void Add(Country aggregate, CancellationToken ct = default);
    Task<Country?> GetByIdAsync(int countryId, CancellationToken ct = default);
    Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken ct = default);
}
