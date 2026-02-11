using Application.Repositories;
using Application.Services.Reports.DTOs;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Policy = Infrastructure.Persistence.Models.Policy;

namespace Infrastructure.Persistence.Repositories;

public sealed class ReportQuery(InsuranceDbContext dbContext) : IReportQuery
{
    public async Task<IReadOnlyList<ReportRow>> GetByCountryAsync(GetReportsCriteria filter, CancellationToken ct)
    {
        var query = GetFilteredQueryByCriteria(filter);
        var reportByCountry = await query
                              .Select(p => new
                              {
                                  CountryId = p.BuildingKeyNavigation.City.County.CountryId,
                                  CurrencyCode = p.CurrencyCode,
                                  p.FinalPremiumAmount
                              })
                              .GroupBy(x => new { x.CountryId, x.CurrencyCode })
                              .Select(g => new ReportRow(
                                g.Key.CountryId.ToString(),
                                g.Key.CurrencyCode,
                                g.Count(),
                                g.Sum(x => x.FinalPremiumAmount)
                              ))
                              .ToListAsync(ct);

        return reportByCountry;
    }

    public async Task<IReadOnlyList<ReportRow>> GetByCountyAsync(GetReportsCriteria filter, CancellationToken ct)
    {
        var query = GetFilteredQueryByCriteria(filter);
        var reportByCounty = await query
                                   .Select(p => new
                                    {
                                        CountyId = p.BuildingKeyNavigation.City.CountyId,
                                        CurrencyCode = p.CurrencyCode,
                                        p.FinalPremiumAmount
                                    })
                                   .GroupBy(x => new { x.CountyId, x.CurrencyCode })
                                   .Select(g => new ReportRow(
                                       g.Key.CountyId.ToString(),
                                       g.Key.CurrencyCode,
                                       g.Count(),
                                       g.Sum(x => x.FinalPremiumAmount)
                                    ))
                                   .ToListAsync(ct);
    
        return reportByCounty;
    }

    public async Task<IReadOnlyList<ReportRow>> GetByCityAsync(GetReportsCriteria filter, CancellationToken ct)
    {
        var query = GetFilteredQueryByCriteria(filter);
        var reportByCity = await query
                                   .Select(p => new
                                   {
                                       CityId = p.BuildingKeyNavigation.CityId,
                                       CurrencyCode = p.CurrencyCode,
                                       p.FinalPremiumAmount
                                   })
                                   .GroupBy(x => new { x.CityId, x.CurrencyCode })
                                   .Select(g => new ReportRow(
                                       g.Key.CityId.ToString(),
                                       g.Key.CurrencyCode,
                                       g.Count(),
                                       g.Sum(x => x.FinalPremiumAmount)
                                    ))
                                   .ToListAsync(ct);

        return reportByCity;

    }

    public async Task<IReadOnlyList<ReportRow>> GetByBrokerAsync(GetReportsCriteria filter, CancellationToken ct)
    {
        var query = GetFilteredQueryByCriteria(filter);
        var reportByBroker = await query.Select(p => new
                                            { p.BrokerKeyNavigation.Code,
                                              p.CurrencyCode,
                                              p.FinalPremiumAmount
                                            })
                                        .GroupBy(x => new { x.Code, x.CurrencyCode })
                                        .Select(g => new ReportRow(
                                           g.Key.Code,
                                           g.Key.CurrencyCode,
                                           g.Count(),
                                           g.Sum(x => x.FinalPremiumAmount)
                                        )).ToListAsync(ct);

        return reportByBroker;
    }

    private IQueryable<Policy> GetFilteredQueryByCriteria(GetReportsCriteria filter)
    {
        var query = dbContext.Policies.AsNoTracking()
                      .Where(p => p.StartDate >= filter.from &&
                                  p.StartDate <= filter.to);

        if (filter.policyStatus is not null)
            query = query.Where(p => p.Status == filter.policyStatus.ToString());

        if (filter.buildingType is not null)
            query = query.Where(p => p.BuildingKeyNavigation.BuildingType == filter.buildingType.ToString());

        if (filter.currencyCode is not null)
            query = query.Where(p => p.CurrencyCode == filter.currencyCode);

        return query;
    }
}
