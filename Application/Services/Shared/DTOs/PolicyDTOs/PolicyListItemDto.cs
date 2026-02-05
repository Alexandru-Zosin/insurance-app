using Domain.Policies;
namespace Application.Services.Shared.DTOs.PolicyDTOs;

public sealed record PolicyListItemDto(
    Guid Id,
    PolicyStatus Status,
    ValidityPeriodDto Tenure)
{
    public static PolicyListItemDto From(Policy e) =>
        new(e.Number, e.Status, ValidityPeriodDto.From(e.Tenure));
}
