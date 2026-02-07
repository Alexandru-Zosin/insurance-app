using Domain.Clients;
namespace Application.Services.Shared.DTOs.ClientDTOs;
public sealed record ClientListItemDto(Guid Id, string Name)
{
    public static ClientListItemDto From(Client e) =>
       new(e.Id, e.Name);
}

