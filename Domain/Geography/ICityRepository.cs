namespace Domain.Geography;

public interface ICityRepository
{
    Task<IReadOnlyList<City>> GetByCountyIdAsync(int countyId, CancellationToken cancellationToken = default);
    Task<City?> GetByIdAsync(int cityId, CancellationToken cancellationToken = default);
}
