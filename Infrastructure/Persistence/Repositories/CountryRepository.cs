using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCountry = Infrastructure.Persistence.Models.Country;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountryRepository(InsuranceDbContext dbContext) : ICountryRepository
{
    public void Add(Country countryToAdd)
    {
        if (countryToAdd is null) 
            throw new ArgumentNullException(nameof(countryToAdd));

        var countryRow = MapToEf(countryToAdd);

        dbContext.Countries.Add(countryRow);
    }

    public async Task<Country?> GetByIdAsync(int countryId, CancellationToken ct = default)
    {
        if (countryId <= 0) 
            throw new ArgumentOutOfRangeException(nameof(countryId));

        var countryRow = await dbContext.Countries
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.CountryId == countryId, ct);

        return countryRow is null ? null : MapToDomain(countryRow);
    }

    public async Task<IReadOnlyList<Country>> ListAsync(CancellationToken ct = default)
    {
        var countryRows = await dbContext.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

        return countryRows.Select(MapToDomain).ToList();
    }

    private static EfCountry MapToEf(Country country)
    {
        return new EfCountry
        {
            CountryId = country.Id,
            Name = country.Name
        };
    }

    private static Country MapToDomain(EfCountry countryRow)
    {
        return Country.Create(
            id: countryRow.CountryId,
            name: countryRow.Name);
    }
}
