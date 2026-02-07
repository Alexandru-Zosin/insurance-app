using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public interface IRiskConfigurationMapperRegistry
{
    IRiskConfigurationMapper ResolveForAggregate(IRiskConfiguration aggregate);
    IRiskConfigurationMapper ResolveForRow(PremiumRule row);
    bool TryResolveForRow(PremiumRule row, out IRiskConfigurationMapper mapper);
}