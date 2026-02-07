using Domain.Configurations;
namespace Application.Services.Shared.DTOs.MetadataDTOs;

public sealed record FeeConfigurationListDto(
  Guid Id,
  string Name,
  decimal Percentage,
  bool IsActive)
{
    public static FeeConfigurationListDto From(FeeConfiguration e) =>
        new(e.Id, e.Name, e.Percentage, e.IsActive);
}
