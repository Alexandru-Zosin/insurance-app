using Domain.Buildings;

namespace Application.Services.Shared.DTOs.BuildingDTOs;

public sealed record BuildingCoreDto(
 Guid OwnerClientId,
 AddressDto Address,
 int CityId,
 int ConstructionYear,
 BuildingType BuildingType,
 int SurfaceArea,
 MoneyDto InsuredValue,
 IReadOnlyList<RiskTagDto> RiskTags)
{
    public static BuildingCoreDto From(Building e) =>
        new(
            e.OwnerClientId,
            AddressDto.From(e.Address),
            e.CityId,
            e.ConstructionYear,
            e.BuildingType,
            e.SurfaceArea,
            MoneyDto.From(e.InsuredValue),
            e.RiskTags.Select(RiskTagDto.From).ToArray()
        );
}