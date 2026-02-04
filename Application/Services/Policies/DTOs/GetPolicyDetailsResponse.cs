using Application.Services.Shared.DTOs.Policy;

namespace Application.Services.Policies.DTOs
{
    public sealed record GetPolicyDetailsResponse(
        PolicyDetailsDto Data
    );
}
