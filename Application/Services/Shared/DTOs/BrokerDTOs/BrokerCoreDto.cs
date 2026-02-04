using Domain.Brokers;

namespace Application.Services.Shared.DTOs.BrokerDTOs;

public sealed record BrokerCoreDto(
    string BrokerCode,
    string Name,
    ContactInfoDto Contact,
    bool IsActive,
    decimal? CommissionPercentage)
{
    public static BrokerCoreDto From(Broker e) =>
       new(
           e.Code,
           e.Name,
           ContactInfoDto.From(e.ContactInfo),
           e.IsActive,
           e.CommissionPercentage);
}