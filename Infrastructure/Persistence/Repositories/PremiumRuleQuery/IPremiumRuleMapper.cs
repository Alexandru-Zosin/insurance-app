using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.PremiumRuleQuery;

public interface IPremiumRuleMapper
{
    bool CanMaterialize(PremiumRule row);
    IPremiumRule ToDomain(PremiumRule row);
}
