using Domain.Configurations;

namespace Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;

public interface IRiskCategoryResolver
{
    int ToId(ZoneRiskCategory category);
    ZoneRiskCategory ToEnum(int id);
}
