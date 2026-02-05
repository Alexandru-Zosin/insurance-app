namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record RiskListItemDto(
    Guid Id,
    string Name,
    decimal Percentage,
    bool IsActive,
    string Kind
)
{
};
