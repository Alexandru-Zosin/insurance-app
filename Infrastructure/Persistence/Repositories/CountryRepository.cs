using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountryRepository : ICountryRepository
{
    private readonly InsuranceDbContext _db;

    public CountryRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<Country> GetAll()
    {
        return _db.Countries
            .AsNoTracking()
            .Select(c => new Country(c.CountryId, c.Name))
            .ToList();
    }

    public Country? GetById(int countryId)
    {
        var ef = _db.Countries
            .AsNoTracking()
            .SingleOrDefault(c => c.CountryId == countryId);

        return ef == null ? null : new Country(ef.CountryId, ef.Name);
    }
}
