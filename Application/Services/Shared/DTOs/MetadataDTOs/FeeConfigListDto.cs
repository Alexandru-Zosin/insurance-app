using Domain.Configurations;
namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record FeeConfigListItemDto(
  Guid Id,
  string Name,
  decimal Percentage,
  bool IsActive)
{
    public static FeeConfigListItemDto From(FeeConfiguration e) =>
        new(e.Id, e.Name, e.Percentage, e.IsActive);
}
