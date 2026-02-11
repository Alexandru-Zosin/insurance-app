using Domain.Geography;
namespace Application.Repositories;

public interface ICityRepository
{
    void Add(City city);
    Task<City?> GetByIdAsync(int cityId, CancellationToken ct = default);
    Task<IReadOnlyList<City>> ListByCountyIdAsync(int countyId, CancellationToken ct = default);
}
