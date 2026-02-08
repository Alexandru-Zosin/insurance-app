using Domain.Geography;
namespace Application.Repositories;

public interface ICityRepository
{
    void Add(City aggregate, CancellationToken ct = default);
    Task<City?> GetByIdAsync(int cityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<City>> GetByCountyIdAsync(int countyId, CancellationToken ct = default);
}
