using Application.Services.Shared.DTOs.ClientDTOs;
namespace Application.Services.Clients.DTOs;

public sealed record GetClientDetailsResponse(ClientDetailedDto Client);