using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCity = Infrastructure.Persistence.Models.City;

namespace Infrastructure.Persistence.Repositories;

public sealed class CityRepository(InsuranceDbContext _db) : ICityRepository
{
    public async Task AddAsync(City aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var ef = ToEfModel(aggregate);

        _db.Cities.Add(ef);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<City?> GetByIdAsync(int cityId, CancellationToken cancellationToken = default)
    {
        if (cityId <= 0) throw new ArgumentOutOfRangeException(nameof(cityId));

        var ef = await _db.Cities
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.CityId == cityId, cancellationToken)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<City>> GetByCountyIdAsync(int countyId, CancellationToken ct = default)
    {
        if (countyId <= 0) throw new ArgumentOutOfRangeException(nameof(countyId));

        var rows = await _db.Cities
            .AsNoTracking()
            .Where(c => c.CountyId == countyId)
            .OrderBy(c => c.Name)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    private static EfCity ToEfModel(City domain)
    {
        return new EfCity
        {
            CityId = domain.Id,
            CountyId = domain.CountyId,
            Name = domain.Name
        };
    }

    private static City ToDomain(EfCity ef)
    {
        return City.Create(
            id: ef.CityId,
            countyId: ef.CountyId,
            name: ef.Name);
    }
}
