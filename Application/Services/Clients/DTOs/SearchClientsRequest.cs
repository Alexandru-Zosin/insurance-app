namespace Application.Services.Clients.DTOs;

public sealed record SearchClientsRequest(
    string? Name,
    string? Identifier);
