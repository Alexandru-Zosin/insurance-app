using Domain.Policies;
using Domain.Shared;

namespace Application.Services.Policies.DTOs;

public sealed record CreateDraftPolicyResponse(
    Guid PolicyNumber,
    PolicyStatus Status,
    Guid ClientId,
    Guid BuildingId,
    Guid BrokerId,
    ValidityPeriod Tenure,
    Money BasePremium,
    string CurrencyCode,
    Money PreliminaryFinalPremium);

