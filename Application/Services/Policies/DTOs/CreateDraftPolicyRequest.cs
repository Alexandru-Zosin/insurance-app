using Domain.Shared;

namespace Application.Services.Policies.DTOs;

public sealed record CreateDraftPolicyRequest(
    Guid BrokerId,
    Guid ClientId,
    Guid BuildingId,
    string CurrencyCode,
    Money BasePremium,
    ValidityPeriod Tenure
);
