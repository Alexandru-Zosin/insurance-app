namespace Application.Services.Metadata.DTOs;

public sealed record UpdateRiskRequest(Guid RiskId, string Name, decimal Percentage);
