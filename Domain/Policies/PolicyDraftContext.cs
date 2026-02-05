using Domain.Buildings;
using Domain.Shared;

namespace Domain.Policies;

public sealed record PolicyDraftContext(
    Guid BrokerId,
    decimal? BrokerCommissionPercentage,
    int? CountryId,
    int? CountyId,
    int? CityId,
    BuildingType BuildingType,
    IReadOnlyCollection<RiskTag> BuildingRiskTags,
    Money BasePremium,
    DateOnly DraftDate
);
