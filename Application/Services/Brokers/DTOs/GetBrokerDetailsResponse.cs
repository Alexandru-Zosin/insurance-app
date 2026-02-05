using Application.Services.Shared.DTOs.BrokerDTOs;

namespace Application.Services.Brokers.DTOs;

public sealed record GetBrokerDetailsResponse(BrokerDetailedDto Broker);
