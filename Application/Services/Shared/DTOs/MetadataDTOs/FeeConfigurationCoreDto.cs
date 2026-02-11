using Domain.Configurations;
namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record FeeConfigurationCoreDto(
    string Name,
    FeeType Type,
    decimal Percentage,
    ValidityPeriodDto ValidityPeriod,
    bool IsActive)
{
    public static FeeConfigurationCoreDto From(FeeConfiguration e) =>
        new(e.Name, e.Type, e.Percentage, ValidityPeriodDto.From(e.ValidityPeriod), e.IsActive);
}
