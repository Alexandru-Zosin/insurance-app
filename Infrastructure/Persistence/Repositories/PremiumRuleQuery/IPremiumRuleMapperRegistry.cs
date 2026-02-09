using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.PremiumRuleQuery;

public interface IPremiumRuleMapperRegistry
{
    IPremiumRuleMapper ResolveMapperForRow(PremiumRule ruleRow);
}
