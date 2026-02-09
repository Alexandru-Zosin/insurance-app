using Domain.Geography;
namespace Application.Repositories;
public interface ICountyRepository
{
    void Add(County county, CancellationToken ct = default);
    Task<County?> GetByIdAsync(int countyId, CancellationToken ct = default);
    Task<IReadOnlyList<County>> GetByCountryIdAsync(int countryId, CancellationToken ct = default);
}
