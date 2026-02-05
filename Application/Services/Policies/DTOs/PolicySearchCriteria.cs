using Domain.Policies;

namespace Application.Services.Policies.DTOs;

public sealed record PolicySearchCriteria(
    Guid? ClientId,
    Guid? BrokerId,
    PolicyStatus? Status,
    DateOnly? StartDate,
    DateOnly? EndDate
);
