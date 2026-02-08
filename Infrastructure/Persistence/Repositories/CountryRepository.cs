using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCountry = Infrastructure.Persistence.Models.Country;

namespace Infrastructure.Persistence.Repositories;

public sealed class CountryRepository(InsuranceDbContext _db) : ICountryRepository
{
    public void Add(Country aggregate, CancellationToken ct = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var ef = ToEfModel(aggregate);

        _db.Countries.Add(ef);
    }

    public async Task<Country?> GetByIdAsync(int countryId, CancellationToken cancellationToken = default)
    {
        if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));

        var ef = await _db.Countries
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.CountryId == countryId, cancellationToken)
            .ConfigureAwait(false);

        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rows.Select(ToDomain).ToList();
    }

    private static EfCountry ToEfModel(Country domain)
    {
        return new EfCountry
        {
            CountryId = domain.Id,
            Name = domain.Name
        };
    }

    private static Country ToDomain(EfCountry ef)
    {
        return Country.Create(
            id: ef.CountryId,
            name: ef.Name);
    }
}
