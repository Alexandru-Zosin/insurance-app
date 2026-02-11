namespace Application.Services.Reports.DTOs;

public sealed record ReportRow(
    string Id,
    string CurrencyCode,
    int PolicyCount,
    decimal TotalFinalPremium
);

