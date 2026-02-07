using Domain.Brokers;
namespace Application.Services.Shared.DTOs.BrokerDTOs;

public sealed record BrokerListItemDto(Guid Id, string Name)
{
    public static BrokerListItemDto From(Broker e) => new(e.Id, e.Name);
}

