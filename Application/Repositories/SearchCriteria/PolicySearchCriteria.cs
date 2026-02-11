using Domain.Policies;

namespace Application.Repositories.SearchCriteria;

public sealed record PolicySearchCriteria(
    Guid? ClientId,
    Guid? BrokerId,
    Guid? BuildingId,
    PolicyStatus? Status,
    DateOnly? StartDate,
    DateOnly? EndDate
)
{
    public static PolicySearchCriteria ByClientId(Guid clientId) =>
        new(clientId, null, null, null, null, null);

    public static PolicySearchCriteria ByBrokerId(Guid brokerId) =>
        new(null, brokerId, null, null, null, null);

    public static PolicySearchCriteria ByBuildingId(Guid buildingId) =>
        new(null, null, buildingId, null, null, null);
};
