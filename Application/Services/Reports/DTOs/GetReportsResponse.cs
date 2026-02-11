namespace Application.Services.Reports.DTOs;

public sealed record GetReportsResponse(IReadOnlyList<ReportRow> Reports);