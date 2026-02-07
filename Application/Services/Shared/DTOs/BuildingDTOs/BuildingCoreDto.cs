using Domain.Buildings;
using Domain.Configurations;

namespace Application.Services.Shared.DTOs.BuildingDTOs;

public sealed record BuildingCoreDto(
 Guid OwnerClientId,
 AddressDto Address,
 int CityId,
 int ConstructionYear,
 BuildingType BuildingType,
 int SurfaceArea,
 MoneyDto InsuredValue,
 IReadOnlyList<ZoneRiskCategory> ZoneRiskCategories)
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
            e.ZoneRiskCategories.ToArray()
        );
}