using Domain.Geography;
using Domain.Policies;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.AddDbContext<InsuranceDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("InsuranceDb")));

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICountyRepository, CountyRepository>();
        services.AddScoped<ICityRepository, CityRepository>();

        return services;
    }
}
