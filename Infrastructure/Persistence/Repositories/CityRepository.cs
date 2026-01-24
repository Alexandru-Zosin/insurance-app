using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class CityRepository : ICityRepository
{
    private readonly InsuranceDbContext _db;

    public CityRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<City> GetByCountyId(int countyId)
    {
        return _db.Cities
            .AsNoTracking()
            .Where(c => c.CountyId == countyId)
            .Select(c => new City(c.CityId, c.Name))
            .ToList();
    }

    public City? GetById(int cityId)
    {
        var ef = _db.Cities
            .AsNoTracking()
            .SingleOrDefault(c => c.CityId == cityId);

        return ef == null ? null : new City(ef.CityId, ef.Name);
    }
}
