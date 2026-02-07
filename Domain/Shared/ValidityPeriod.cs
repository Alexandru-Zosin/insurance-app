using Domain.Common;
namespace Domain.Shared;

public sealed record ValidityPeriod(DateOnly StartDate, DateOnly EndDate)
{
    public static ValidityPeriod Create(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            throw new DomainException("End Date is before Start Date.");

        return new ValidityPeriod(startDate, endDate);
    }

    public static ValidityPeriod? CreateOptional(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate is null || endDate is null)
            return null;

        return Create(startDate.Value, endDate.Value);
    }

    public bool Contains(DateOnly date) => StartDate <= date && date <= EndDate;
}