namespace Domain.Geography;

public interface ICountryRepository
{
    IReadOnlyList<Country> GetAll();
    Country? GetById(int countryId);
}
