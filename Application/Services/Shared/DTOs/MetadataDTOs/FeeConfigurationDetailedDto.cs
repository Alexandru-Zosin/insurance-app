using Domain.Configurations;

namespace Application.Services.Shared.DTOs.MetadataDTOs
{
    public sealed record FeeConfigurationDetailedDto(
    Guid Id,
    FeeConfigurationCoreDto Core)
    {
        public static FeeConfigurationDetailedDto From(FeeConfiguration e) =>
            new(e.Id, FeeConfigurationCoreDto.From(e));
    }
}
