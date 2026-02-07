using Domain.Configurations;

namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record RiskConfigurationListItemDto(
    Guid Id,
    RiskConfigurationCoreDto RiskConfigurationCore
)
{
    public static RiskConfigurationListItemDto From(IRiskConfiguration r)
        => new(r.Core.Id, new RiskConfigurationCoreDto(r.Core.Name, r.Core.Percentage, r.Core.IsActive));
};
