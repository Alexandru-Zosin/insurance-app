using Domain.Policies;

namespace Application.Services.Policies.DTOs;

public sealed record ActivatePolicyResponse(
    Guid PolicyNumber,
    PolicyStatus Status
);
