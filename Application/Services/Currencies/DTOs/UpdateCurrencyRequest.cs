using Application.Services.Shared.DTOs.CurrencyDTOs;
namespace Application.Services.Currencies.DTOs;

public sealed record UpdateCurrencyRequest(string CurrencyCode, CurrencyUpdateDto Currency);
