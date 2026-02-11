using Domain.Configurations;

namespace Application.Services.Policies
{
    public interface IPremiumRulesProvider
    {
        Task<IReadOnlyList<IPremiumRule>> GetActiveRulesAsync(CancellationToken ct = default);
    }
}