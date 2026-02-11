using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCounty = Infrastructure.Persistence.Models.County;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountyRepository(InsuranceDbContext dbContext) : ICountyRepository
{
    public void Add(County countyToAdd, CancellationToken ct = default)
    {
        if (countyToAdd is null) throw new ArgumentNullException(nameof(countyToAdd));

        var countyRow = MapToEf(countyToAdd);

        dbContext.Counties.Add(countyRow);
    }

    public async Task<County?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

        var countyRow = await dbContext.Counties
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.CountyId == id, ct);

        return countyRow is null ? null : MapToDomain(countyRow);
    }

    public async Task<IReadOnlyList<County>> GetByCountryIdAsync(int countryId, CancellationToken ct = default)
    {
        if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));

        var countyRows = await dbContext.Counties
            .AsNoTracking()
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return countyRows.Select(MapToDomain).ToList();
    }

    private static EfCounty MapToEf(County county)
    {
        return new EfCounty
        {
            CountyId = county.Id,
            CountryId = county.CountryId,
            Name = county.Name
        };
    }

    private static County MapToDomain(EfCounty countyRow)
    {
        return County.Create(
            id: countyRow.CountyId,
            countryId: countyRow.CountryId,
            name: countyRow.Name);
    }
}
