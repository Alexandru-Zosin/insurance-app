namespace Domain.Geography;

public interface ICountryRepository
{
    Country? GetById(int countryId);
    IReadOnlyList<Country> GetAll();
}
