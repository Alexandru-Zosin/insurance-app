using Domain.Buildings;
using Domain.Shared;

namespace Domain.Policies;

public sealed record PolicyDraftContext(
    Guid BrokerId,
    int? CountryId,
    int? CountyId,
    int? CityId,
    BuildingType BuildingType,
    Money BasePremium,
    DateOnly DraftDate
);
