using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.PremiumRuleQuery;

public interface IPremiumRuleMapper
{
    bool CanMap(PremiumRule ruleRow);
    IPremiumRule MapToDomain(PremiumRule ruleRow);
}
