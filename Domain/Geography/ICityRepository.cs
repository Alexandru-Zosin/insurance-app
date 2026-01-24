namespace Domain.Geography;

public interface ICityRepository
{
    IReadOnlyList<City> GetByCountyId(int countyId);
    City? GetById(int cityId);
}
