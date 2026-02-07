using Domain.Configurations;
using Infrastructure.Persistence.Models;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public interface IRiskConfigurationMapper
{
    bool CanMaterialize(PremiumRule row);
    IRiskConfiguration ToDomain(PremiumRule row);
    bool CanPersist(IRiskConfiguration aggregate);
    PremiumRule ToEfModel(IRiskConfiguration aggregate);
    void UpdateEfModel(PremiumRule row, IRiskConfiguration aggregate);
}
