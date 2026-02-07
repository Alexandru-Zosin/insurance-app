namespace Application.Services.Metadata.DTOs;

public sealed record CreateCountyRiskRequest(string Name, decimal Percentage, bool IsActive, int CountyId);
