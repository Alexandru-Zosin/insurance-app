namespace Application.Services.Clients.DTO;

public sealed record SearchClientsRequest(
    string? Name,
    string? Identifier);
