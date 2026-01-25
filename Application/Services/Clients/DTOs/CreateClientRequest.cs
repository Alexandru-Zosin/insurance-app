namespace Application.Services.Clients.DTO;

public sealed record CreateClientRequest(
    string ClientType,
    string Name,
    string RegistrationNumber,
    string Email,
    string Phone,
    string? Address);
