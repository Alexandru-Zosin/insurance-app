using Domain.Geography;
namespace Application.Repositories;
public interface ICountyRepository
{
    void Add(County county);
    Task<County?> GetByIdAsync(int countyId, CancellationToken ct = default);
    Task<IReadOnlyList<County>> ListByCountryIdAsync(int countryId, CancellationToken ct = default);
}
