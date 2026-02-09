using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public interface IRiskConfigurationMapper
{
    bool CanMap(PremiumRule row);
    IRiskConfiguration MapToDomain(PremiumRule row);
    bool CanPersist(IRiskConfiguration aggregate);
    PremiumRule MapToEf(IRiskConfiguration aggregate);
    void MapOntoEf(PremiumRule row, IRiskConfiguration aggregate);
}
