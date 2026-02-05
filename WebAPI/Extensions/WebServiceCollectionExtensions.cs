using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

namespace WebAPI.Extensions;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterBuildingRequestValidator>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(cfg =>
        {
            cfg.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Insurance API",
                Version = "v1",
                Description = "Broker-facing insurance management API"
            });

            cfg.CustomSchemaIds(type => type.FullName);
        });

        return services;

    }
}
