using Domain.Geography;
using Application.Repositories;
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

    public async Task<IReadOnlyList<County>> GetByCountryIdAsync(
        int countryId,
        CancellationToken cancellationToken)
    {
        return await _db.Counties
            .AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .Select(c => new County(c.CountyId, c.Name))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
