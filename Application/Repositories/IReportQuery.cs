using Application.Services.Reports.DTOs;
namespace Application.Repositories;

public interface IReportQuery
{
    Task<IReadOnlyList<ReportRow>> GetByCountryAsync(GetReportsCriteria filter,
        CancellationToken ct);
    Task<IReadOnlyList<ReportRow>> GetByCountyAsync(GetReportsCriteria filter, CancellationToken ct);
    Task<IReadOnlyList<ReportRow>> GetByCityAsync(GetReportsCriteria filter, CancellationToken ct);
    Task<IReadOnlyList<ReportRow>> GetByBrokerAsync(GetReportsCriteria filter, CancellationToken ct);
}
