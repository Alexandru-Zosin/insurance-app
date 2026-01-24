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

    public async Task<IReadOnlyList<City>> GetByCountyIdAsync(
        int countyId,
        CancellationToken cancellationToken)
    {
        return await _db.Cities
            .AsNoTracking()
            .Where(c => c.CountyId == countyId)
            .Select(c => new City(c.CityId, c.Name))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<City?> GetByIdAsync(
        int cityId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Cities
            .AsNoTracking()
            .SingleOrDefaultAsync(
                c => c.CityId == cityId,
                cancellationToken)
            .ConfigureAwait(false);

        return ef == null ? null : new City(ef.CityId, ef.Name);
    }
}
