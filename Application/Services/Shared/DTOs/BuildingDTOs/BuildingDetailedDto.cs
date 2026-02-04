using Application.Services.Shared.DTOs.PolicyDTOs;
using Domain.Buildings;

namespace Application.Services.Shared.DTOs.BuildingDTOs;

public sealed record BuildingDetailedDto(
    Guid Id,
    BuildingCoreDto Core,
    IReadOnlyList<PolicyListItemDto> Policies
)
{
    public static BuildingDetailedDto From(
       Building e,
       IReadOnlyList<PolicyListItemDto> policies) =>
       new(e.Id, BuildingCoreDto.From(e), policies);
}
