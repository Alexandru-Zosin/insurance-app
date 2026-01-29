namespace Application.Services.Clients.DTOs;

public sealed record CreateClientRequest(
    string ClientType,
    string Name,
    string RegistrationNumber,
    string Email,
    string Phone,
    string Street,
    string Number
    );
