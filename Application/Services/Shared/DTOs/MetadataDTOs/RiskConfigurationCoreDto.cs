using Domain.Configurations;

namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record RiskConfigurationCoreDto(
    string Name,
    decimal Percentage,
    bool IsActive
)
{
    public static RiskConfigurationCoreDto From(IRiskConfiguration r)
        => new(r.Core.Name, r.Core.Percentage, r.Core.IsActive);
}
