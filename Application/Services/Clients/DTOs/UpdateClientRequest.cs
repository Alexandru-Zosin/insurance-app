namespace Application.Services.Clients.DTOs;

public sealed record UpdateClientRequest(
    Guid ClientId,
    string Name,
    string Email,
    string Phone,
    string Street,
    string Number);
