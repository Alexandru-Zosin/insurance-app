using Domain.Shared;
namespace Application.Services.Shared.DTOs;

public sealed record ValidityPeriodDto(DateOnly StartDate, DateOnly EndDate)
{
    public static ValidityPeriodDto From(ValidityPeriod p) =>
         new(p.StartDate, p.EndDate);

    public ValidityPeriod ToDomain() => new(StartDate, EndDate);
}
