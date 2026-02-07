using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCounty = Infrastructure.Persistence.Models.County;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountyRepository(InsuranceDbContext _db) : ICountyRepository
{
    public async Task AddAsync(County aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var ef = ToEfModel(aggregate);

        _db.Counties.Add(ef);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<County?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

        var ef = await _db.Counties
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.CountyId == id, ct)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<County>> GetByCountryIdAsync(int countryId, CancellationToken ct = default)
    {
        if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));

        var rows = await _db.Counties
            .AsNoTracking()
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    private static EfCounty ToEfModel(County domain)
    {
        return new EfCounty
        {
            CountyId = domain.Id,
            CountryId = domain.CountryId,
            Name = domain.Name
        };
    }

    private static County ToDomain(EfCounty ef)
    {
        return County.Create(
            id: ef.CountyId,
            countryId: ef.CountryId,
            name: ef.Name);
    }
}
