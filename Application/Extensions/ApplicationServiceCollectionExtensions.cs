using Application.Services.Buildings;
using Application.Services.Buildings.GetBuildingsForClient;
using Application.Services.Buildings.UpdateBuilding;
using Application.UseCases.Clients;
using Application.UseCases.Geography;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Clients
        services.AddScoped<SearchClientsService>();
        services.AddScoped<GetClientDetailsService>();
        services.AddScoped<CreateClientService>();
        services.AddScoped<UpdateClientService>();

        // Buildings
        services.AddScoped<GetBuildingsForClientService>();
        services.AddScoped<GetBuildingDetailsService>();
        services.AddScoped<RegisterBuildingService>();
        services.AddScoped<UpdateBuildingService>();

        // Geography
        services.AddScoped<GetCountriesService>();
        services.AddScoped<GetCountiesByCountryService>();
        services.AddScoped<GetCitiesByCountyService>();

        return services;
    }
}

