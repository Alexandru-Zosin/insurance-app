namespace Application.Services.Policies.DTOs;

public sealed record ActivatePolicyRequest(
    Guid BrokerId
);
