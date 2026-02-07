using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.PremiumRuleQuery;

public interface IPremiumRuleMapperRegistry
{
    IPremiumRuleMapper ResolveForRow(PremiumRule row);
}
