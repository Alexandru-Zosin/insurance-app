using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountyRepository : ICountyRepository
{
    private readonly InsuranceDbContext _db;

    public CountyRepository(InsuranceDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<County> GetByCountryId(int countryId)
    {
        return _db.Counties
            .AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .Select(c => new County(c.CountyId, c.Name))
            .ToList();
    }
}
