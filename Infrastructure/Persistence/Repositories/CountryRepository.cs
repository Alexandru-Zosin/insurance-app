using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCountry = Infrastructure.Persistence.Models.Country;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountryRepository(InsuranceDbContext _dbContext) : ICountryRepository
{
    public void Add(Country countryToAdd, CancellationToken ct = default)
    {
        if (countryToAdd is null) throw new ArgumentNullException(nameof(countryToAdd));

        var countryRow = MapToEf(countryToAdd);

        _dbContext.Countries.Add(countryRow);
    }

    public async Task<Country?> GetByIdAsync(int countryId, CancellationToken cancellationToken = default)
    {
        if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));

        var countryRow = await _dbContext.Countries
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.CountryId == countryId, cancellationToken);

        return countryRow is null ? null : MapToDomain(countryRow);
    }

    public async Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var countryRows = await _dbContext.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

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
