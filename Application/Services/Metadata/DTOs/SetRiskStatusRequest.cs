namespace Application.Services.Metadata.DTOs;

public sealed record SetRiskStatusRequest(Guid RiskId, bool Active);
