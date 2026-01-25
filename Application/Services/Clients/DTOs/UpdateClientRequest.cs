namespace Application.Services.Clients.DTO;

public sealed record UpdateClientRequest(
    Guid ClientId,
    string Name,
    string Email,
    string Phone,
    string? Address);
