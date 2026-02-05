using Domain.Brokers;

namespace Application.Services.Shared.DTOs.BrokerDTOs;

public sealed record BrokerDetailedDto(
     Guid Id,
     BrokerCoreDto Core)
{
    public static BrokerDetailedDto From(Broker e) =>
        new(e.Id, BrokerCoreDto.From(e));
}