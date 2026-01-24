namespace Domain.Geography;

public interface ICountyRepository
{
    IReadOnlyList<County> GetByCountryId(int countryId);

}
