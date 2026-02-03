using Domain.Geography;
using Application.Repositories;
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

    public async Task<IReadOnlyList<Country>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _db.Countries
            .AsNoTracking()
            .Select(c => new Country(c.CountryId, c.Name))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Country?> GetByIdAsync(
        int countryId,
        CancellationToken cancellationToken)
    {
        var ef = await _db.Countries
            .AsNoTracking()
            .SingleOrDefaultAsync(
                c => c.CountryId == countryId,
                cancellationToken)
            .ConfigureAwait(false);

        return ef == null ? null : new Country(ef.CountryId, ef.Name);
    }
}
