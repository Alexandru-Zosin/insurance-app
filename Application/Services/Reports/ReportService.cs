using Application.Common;
using Application.Repositories;
using Application.Services.Reports.DTOs;

namespace Application.Services.Reports;

public class ReportService(
    IReportQuery reportsQuery
    ) : IReportService
{
    public async Task<Result<GetReportsResponse>> GetReportsByCountryAsync(
        GetReportsCriteria criteria,
        CancellationToken ct = default)
    {
        var reportsByCountry = await reportsQuery.GetByCountryAsync(criteria, ct);

        var response = new GetReportsResponse(reportsByCountry);
        return Result<GetReportsResponse>.Ok(response);
    }

    public async Task<Result<GetReportsResponse>> GetReportsByCountyAsync(
        GetReportsCriteria criteria,
        CancellationToken ct = default)
    {
        var reportsByCounty = await reportsQuery.GetByCountyAsync(criteria, ct);

        var response = new GetReportsResponse(reportsByCounty);
        return Result<GetReportsResponse>.Ok(response);
    }

    public async Task<Result<GetReportsResponse>> GetReportsByCityAsync(
        GetReportsCriteria criteria,
        CancellationToken ct = default)
    {
        var reportsByCity = await reportsQuery.GetByCityAsync(criteria, ct);

        var response = new GetReportsResponse(reportsByCity);
        return Result<GetReportsResponse>.Ok(response);
    }

    public async Task<Result<GetReportsResponse>> GetReportsByBrokerAsync(
        GetReportsCriteria criteria,
        CancellationToken ct = default)
    {
        var reportsByBroker = await reportsQuery.GetByBrokerAsync(criteria, ct);

        var response = new GetReportsResponse(reportsByBroker);
        return Result<GetReportsResponse>.Ok(response);
    }
}
