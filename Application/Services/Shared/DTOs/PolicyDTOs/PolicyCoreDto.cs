using Application.Services.Shared.DTOs.CurrencyDTOs;
using Domain.Policies;
namespace Application.Services.Shared.DTOs.PolicyDTOs;

public sealed record PolicyCoreDto(
    Guid ClientId,
    Guid BuildingId,
    Guid BrokerId,
    ValidityPeriodDto Tenure,
    MoneyDto BasePremium,
    string CurrencyCode,
    MoneyDto? PreliminaryFinalPremium)
{
    public static PolicyCoreDto From(Policy e) =>
       new(
           e.ClientId,
           e.BuildingId,
           e.BrokerId,
           ValidityPeriodDto.From(e.Tenure),
           MoneyDto.From(e.BasePremium),
           e.CurrencyCode,
           MoneyDto.From(e.FinalPremium)
       );
}