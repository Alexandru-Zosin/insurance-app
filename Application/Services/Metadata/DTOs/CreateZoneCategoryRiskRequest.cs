using Domain.Configurations;

namespace Application.Services.Metadata.DTOs;

public sealed record CreateZoneCategoryRiskRequest(string Name, decimal Percentage, bool IsActive, ZoneRiskCategory Category);
