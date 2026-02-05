using Domain.Geography;
namespace Application.Repositories;

public interface ICityRepository
{
    Task AddAsync(City aggregate, CancellationToken ct = default);
    Task<City?> GetByIdAsync(int cityId, CancellationToken cancellationToken = default);
}
