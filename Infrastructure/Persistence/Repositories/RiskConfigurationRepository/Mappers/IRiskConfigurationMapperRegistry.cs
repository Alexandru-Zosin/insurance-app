using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public interface IRiskConfigurationMapperRegistry
{
    IRiskConfigurationMapper ResolveForAggregate(IRiskConfiguration aggregate);
    IRiskConfigurationMapper ResolveMapperForRow(PremiumRule row);
    bool TryResolveMapperForRow(PremiumRule row, out IRiskConfigurationMapper mapper);
}