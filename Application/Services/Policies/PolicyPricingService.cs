using Application.Services.Policies.DTOs;
using Domain.Policies;
using Domain.Services;
using Domain.Shared;

namespace Application.Services.Policies;

public sealed class PolicyPricingService(
    IPremiumRulesProvider premiumRulesProvider,
    IPremiumCalculatorService premiumCalculatorService) : IPolicyPricingService
{
    public async Task<Money> CalculateDraftFinalPremiumAsync(
        PolicyDraftPrerequisites prerequisites,
        CreateDraftPolicyRequest request,
        DateOnly draftDateUtc,
        CancellationToken ct = default)
    {
        var context = new PolicyDraftContext(
            prerequisites.Broker.Id,
            prerequisites.Broker.CommissionPercentage,
            prerequisites.CountryId,
            prerequisites.CountyId,
            prerequisites.CityId,
            prerequisites.Building.BuildingType,
            prerequisites.Building.ZoneRiskCategories,
            request.Policy.BasePremium.MapToDomain(),
            draftDateUtc);

        var rules = await premiumRulesProvider.GetActiveRulesAsync(ct);
        return premiumCalculatorService.CalculateFinalPremium(context, rules);
    }
}
