using Domain.Common;
using Domain.Shared;

namespace Domain.Policies;
public sealed class Policy
{
    public Guid Id { get; }
    public Guid ClientId { get; }
    public Guid BuildingId { get; }
    public Guid BrokerId { get; }
    public Money Premium { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

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

        ValidateInvariants();
    }

    private void ValidateInvariants()
    {
    }

    public static Policy Create(
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        Money premium,
        DateOnly start,
        DateOnly end)
    {

        return new Policy(Guid.NewGuid(), clientId, buildingId, brokerId, premium, start, end);
    }

    public static Policy Rehydrate(
        Guid id,
        Guid clientId,
        Guid buildingId,
        Guid brokerId,
        Money premium,
        DateOnly start,
        DateOnly end)
    {
        return new Policy(id, clientId, buildingId, brokerId, premium, start, end);
    }

    public Policy ChangePeriod(DateOnly start, DateOnly end)
    {
        EnsurePeriod(start, end);
        StartDate = start;
        EndDate = end;
        return this;
    }

    private static void EnsurePeriod(DateOnly start, DateOnly end)
    {
        if (end <= start)
            throw new DomainException("Invalid policy period.");
    }

    public Policy ChangePremium(Money premium)
    {
        Premium = premium;
        return this;
    }
}