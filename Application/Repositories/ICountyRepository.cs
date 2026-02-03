using Domain.Geography;
namespace Application.Repositories;
public interface ICountyRepository
{
    Task<IReadOnlyList<County>> GetByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
}
