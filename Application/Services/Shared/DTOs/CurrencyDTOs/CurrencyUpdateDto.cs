namespace Application.Services.Shared.DTOs.CurrencyDTOs;

public sealed record CurrencyUpdateDto(
     string Name,
     decimal ExchangeRateToBase);
