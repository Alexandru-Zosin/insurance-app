using Domain.Geography;
namespace Application.Repositories;
public interface ICountyRepository
{
    void Add(County aggregate, CancellationToken ct = default);
    Task<County?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<County>> GetByCountryIdAsync(int countryId, CancellationToken ct = default);
}
