using Domain.Clients;
namespace Application.Services.Clients.DTOs;
public sealed record SearchClientsResponse(
    IReadOnlyList<ClientSearchResultDto> Clients);
public sealed record ClientSearchResultDto(
    Guid Id,
    string Type,
    string Name,
    string RegistrationNumber,
    string Email,
    string Phone)
{
    public static ClientSearchResultDto From(Client client)
    {
        return new ClientSearchResultDto(
            client.Id,
            client.Type.ToString(),
            client.Name,
            client.Identifier.Value,
            client.ContactInfo.Email,
            client.ContactInfo.Phone);
    }
}
