namespace Application.Services.Metadata.DTOs;

public sealed record CreateCityRiskRequest(string Name, decimal Percentage, bool IsActive, int CityId);
