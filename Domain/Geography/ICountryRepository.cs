namespace Domain.Geography;

public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Country?> GetByIdAsync(int countryId, CancellationToken cancellationToken = default);
}
