using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public interface IRiskConfigurationMapper
{
    bool CanMapToDomain(PremiumRule row);
    IRiskConfiguration MapToDomain(PremiumRule row);
    bool CanMapToEf(IRiskConfiguration riskConfiguration);
    PremiumRule MapToEf(IRiskConfiguration riskConfiguration);
    void MapOntoEf(PremiumRule row, IRiskConfiguration riskConfiguration);
}
