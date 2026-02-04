namespace Application.Services.Policies.DTOs;

public sealed record CancelPolicyRequest(
    Guid BrokerId,
    Guid PolicyNumber,
    string Reason
);
