using Domain.Common;
using Domain.Shared;

namespace Domain.Policies;
public sealed class Policy
{
    public Guid Id { get; }
    public Guid ClientId { get; }
    public Guid BuildingId { get; }
    public Guid BrokerId { get; }
    public Money Premium { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }

    private Policy(
        Guid id,
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        Money premium,
        DateOnly start,
        DateOnly end)
    {
        Id = id;
        ClientId = clientId;
        BuildingId = buildingId;
        BrokerId = brokerId;
        Premium = premium;
        StartDate = start;
        EndDate = end;
    }

    public static Result<Policy> Issue(
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        Money premium,
        DateOnly start,
        DateOnly end)
    {
        if (end <= start)
            return Result<Policy>.Fail(ErrorType.Validation, "Invalid policy period");

        return Result<Policy>.Ok(
            new Policy(Guid.NewGuid(), clientId, buildingId, brokerId, premium, start, end));
    }
}