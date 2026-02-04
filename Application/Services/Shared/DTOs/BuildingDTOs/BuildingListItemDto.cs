using Domain.Buildings;
namespace Application.Services.Shared.DTOs.BuildingDTOs;

public sealed record BuildingListItemDto(
    Guid Id,
    AddressDto Address
)
{
    public static BuildingListItemDto From(Building e) =>
       new(e.Id, AddressDto.From(e.Address));
}
