using Domain.Buildings;
using Domain.Configurations;
using Domain.Shared;

namespace Domain.Policies;

public sealed record PolicyDraftContext(
    Guid BrokerId,
    decimal? BrokerCommissionPercentage,
    int? CountryId,
    int? CountyId,
    int? CityId,
    BuildingType BuildingType,
    IReadOnlyCollection<ZoneRiskCategory> BuildingZoneRiskCategories,
    Money BasePremium,
    DateOnly DraftDate
);
