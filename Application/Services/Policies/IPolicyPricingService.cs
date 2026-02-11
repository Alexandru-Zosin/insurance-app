using Application.Services.Policies.DTOs;
using Domain.Shared;

namespace Application.Services.Policies
{
    public interface IPolicyPricingService
    {
        Task<Money> CalculateDraftFinalPremiumAsync(PolicyDraftPrerequisites prerequisites, CreateDraftPolicyRequest request, DateOnly draftDateUtc, CancellationToken ct = default);
    }
}