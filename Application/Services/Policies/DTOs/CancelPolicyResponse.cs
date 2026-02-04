using Domain.Policies;
namespace Application.Services.Policies.DTOs;

public sealed record CancelPolicyResponse(
    Guid PolicyNumber,
    PolicyStatus Status,
    string? CancellationReason
);
