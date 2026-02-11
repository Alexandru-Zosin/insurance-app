using Application.Common;
using Application.Services.Reports.DTOs;

namespace Application.Services.Reports
{
    public interface IReportService
    {
        Task<Result<GetReportsResponse>> GetReportsByBrokerAsync(GetReportsCriteria criteria, CancellationToken ct = default);
        Task<Result<GetReportsResponse>> GetReportsByCityAsync(GetReportsCriteria criteria, CancellationToken ct = default);
        Task<Result<GetReportsResponse>> GetReportsByCountryAsync(GetReportsCriteria criteria, CancellationToken ct = default);
        Task<Result<GetReportsResponse>> GetReportsByCountyAsync(GetReportsCriteria criteria, CancellationToken ct = default);
    }
}