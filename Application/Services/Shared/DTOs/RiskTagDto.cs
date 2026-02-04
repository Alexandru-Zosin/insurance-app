using Domain.Shared;
namespace Application.Services.Shared.DTOs;

public sealed record RiskTagDto(RiskCategory Category)
{
    public static RiskTagDto From(RiskTag t) => new(t.RiskCategory);

    public RiskTag ToDomain() => new(Category);
}