using Application.Services.Shared.DTOs.BuildingDTOs;
using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Clients;

namespace Application.Services.Shared.DTOs.ClientDTOs;

public sealed record ClientDetailedDto(
    Guid Id,
    ClientCoreDto ClientInfo,
    IReadOnlyList<BuildingListItemDto> Buildings,
    IReadOnlyList<PolicyListItemDto> Policies
)
{
    public static ClientDetailedDto From(
        Client e,
        IReadOnlyList<BuildingListItemDto> buildings,
        IReadOnlyList<PolicyListItemDto> policies) =>
        new(e.Id, ClientCoreDto.From(e), buildings, policies);
}