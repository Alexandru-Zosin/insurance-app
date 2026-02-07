namespace Application.Services.Metadata.DTOs;

public sealed record CreateCountryRiskRequest(string Name, decimal Percentage, bool IsActive, int CountryId);
