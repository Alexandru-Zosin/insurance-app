using Domain.Buildings;
using Domain.Policies;

namespace Application.Services.Reports.DTOs;

public sealed record GetReportsCriteria(
    DateOnly from,
    DateOnly to,
    PolicyStatus? policyStatus,
    string? currencyCode,
    BuildingType? buildingType
);
