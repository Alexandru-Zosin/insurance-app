using Domain.Configurations;

namespace Application.Repositories;

public interface IPremiumRuleQuery
{
    Task<IReadOnlyList<IPremiumRule>> GetActiveAsync(
        CancellationToken ct = default);
}
