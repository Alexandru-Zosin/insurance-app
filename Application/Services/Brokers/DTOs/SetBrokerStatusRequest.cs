namespace Application.Services.Brokers.DTOs;
public sealed record SetBrokerStatusRequest(Guid BrokerId, bool Active);
