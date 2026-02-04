using Application.Services.Shared.DTOs.BrokerDTOs;

namespace Application.Services.Brokers.DTOs
{
    public sealed record ListBrokersResponse(IReadOnlyList<BrokerListItemDto> Items);
}
