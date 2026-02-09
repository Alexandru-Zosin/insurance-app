using Domain.Geography;
namespace Application.Repositories;

public interface ICityRepository
{
    void Add(City city, CancellationToken ct = default);
    Task<City?> GetByIdAsync(int cityId, CancellationToken ct = default);
    Task<IReadOnlyList<City>> GetByCountyIdAsync(int countyId, CancellationToken ct = default);
}
