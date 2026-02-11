using Application.Repositories;
using Domain.Geography;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EfCity = Infrastructure.Persistence.Models.City;

namespace Infrastructure.Persistence.Repositories;

public sealed class CityRepository(InsuranceDbContext dbContext) : ICityRepository
{
    public void Add(City cityToAdd)
    {
        if (cityToAdd is null) 
            throw new ArgumentNullException(nameof(cityToAdd));

        var cityRow = MapToEf(cityToAdd);

        dbContext.Cities.Add(cityRow);
    }

    public async Task<City?> GetByIdAsync(int cityId, CancellationToken ct = default)
    {
        if (cityId <= 0) 
            throw new ArgumentOutOfRangeException(nameof(cityId));

        var cityRow = await dbContext.Cities
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.CityId == cityId, ct);

        return cityRow is null ? null : MapToDomain(cityRow);
    }

    public async Task<IReadOnlyList<City>> ListByCountyIdAsync(int countyId, CancellationToken ct = default)
    {
        if (countyId <= 0) 
            throw new ArgumentOutOfRangeException(nameof(countyId));

        var cityRows = await dbContext.Cities
            .AsNoTracking()
            .Where(c => c.CountyId == countyId)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

        return cityRows.Select(MapToDomain).ToList();
    }

    private static EfCity MapToEf(City city)
    {
        return new EfCity
        {
            CityId = city.Id,
            CountyId = city.CountyId,
            Name = city.Name
        };
    }

    private static City MapToDomain(EfCity cityRow)
    {
        return City.Create(
            id: cityRow.CityId,
            countyId: cityRow.CountyId,
            name: cityRow.Name);
    }
}
