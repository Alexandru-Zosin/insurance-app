namespace Application.Services.Policies.DTOs;

public sealed record GetPolicyDetailsRequest(
    Guid PolicyNumber
);
