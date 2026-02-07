using Application.Services.Shared.DTOs.CurrencyDTOs;
namespace Application.Services.Currencies.DTOs;

public sealed record AddCurrencyRequest(CurrencyDto Currency);
