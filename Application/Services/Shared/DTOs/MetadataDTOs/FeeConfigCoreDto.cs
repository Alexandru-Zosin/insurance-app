using Domain.Configurations;
namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record FeeConfigCoreDto(
    string Name,
    FeeType Type,
    decimal Percentage,
    ValidityPeriodDto ValidityPeriod,
    bool IsActive)
{
    public static FeeConfigCoreDto From(FeeConfiguration e) =>
        new(e.Name, e.Type, e.Percentage, ValidityPeriodDto.From(e.ValidityPeriod), e.IsActive);
}
