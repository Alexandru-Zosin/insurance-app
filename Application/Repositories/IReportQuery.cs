using Application.Services.Reports.DTOs;
namespace Application.Repositories;

public enum ReportDimension
{
    Country,
    County,
    City,
    Broker
}

public interface IReportQuery
{
    Task<IReadOnlyList<ReportRow>> GetAsync(GetReportsCriteria filter, ReportDimension dimension, CancellationToken ct = default);
}
