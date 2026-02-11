using Application.Common;
using Application.Repositories;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository.Mappers;
using Infrastructure.Persistence.Repositories.RiskConfigurationRepository.MappersRegistry;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.AddDbContext<InsuranceDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("InsuranceDb")));

        services.AddScoped<IBrokerRepository, BrokerRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICountyRepository, CountyRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IFeeConfigurationRepository, FeeConfigurationRepository>();
        services.AddScoped<IRiskConfigurationRepository, RiskConfigurationRepository>();
        services.AddScoped<IReportQuery, ReportQuery>();

        services.AddScoped<IRiskConfigurationMapperRegistry, RiskConfigurationMapperRegistry>();
        services.AddScoped<IRiskConfigurationMapper, CountryRiskConfigurationMapper>();
        services.AddScoped<IRiskConfigurationMapper, CountyRiskConfigurationMapper>();
        services.AddScoped<IRiskConfigurationMapper, CityRiskConfigurationMapper>();
        services.AddScoped<IRiskConfigurationMapper, BuildingTypeRiskConfigurationMapper>();
        services.AddScoped<IRiskConfigurationMapper, ZoneRiskConfigurationMapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
