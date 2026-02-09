using Domain.Geography;
namespace Application.Repositories;
public interface ICountryRepository
{
    void Add(Country aggregate, CancellationToken ct = default);
    Task<Country?> GetByIdAsync(int countryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken cancellationToken = default);
}
