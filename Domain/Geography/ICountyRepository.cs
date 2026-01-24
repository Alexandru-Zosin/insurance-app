namespace Domain.Geography;

public interface ICountyRepository
{
    Task<IReadOnlyList<County>> GetByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
}
