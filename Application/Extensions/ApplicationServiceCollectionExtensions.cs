using Application.Services.Brokers;
using Application.Services.Buildings;
using Application.Services.Clients;
using Application.Services.Currencies;
using Application.Services.Geography;
using Application.Services.Policies;
using Application.Services.Reports;
using Application.Services.Risks;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBrokerService, BrokerService>();
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IGeographyService, GeographyService>();
        services.AddScoped<IFeeConfigurationService, FeeConfigurationService>();
        services.AddScoped<IRiskConfigurationService, RiskConfigurationService>();
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddScoped<IPremiumCalculatorService, PremiumCalculatorService>();

        return services;
    }
}

