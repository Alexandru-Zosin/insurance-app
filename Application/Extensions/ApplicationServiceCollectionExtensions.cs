using Application.Common;
using Application.Services.Buildings;
using Application.Services.Clients;
using Application.Services.Geography;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IGeographyService, GeographyService>();

        return services;
    }
}

