using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using WebAPI.Exceptions;
using WebAPI.Swagger;
using WebAPI.Validators.Buildings;

namespace WebAPI.Extensions;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddWebControllersAndServices(this IServiceCollection services)
    {
        services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterBuildingRequestValidator>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(cfg =>
        {
            cfg.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Insurance API",
                Version = "v1",
                Description = "Broker insurance management API"
            });

            cfg.CustomSchemaIds(type => type.FullName);

            cfg.SchemaFilter<StringEnumSchemaFilterForSwagger>();
        });

        services.AddProblemDetails();

        services.AddSingleton<IExceptionProblemMapper, FluentValidationProblemMapper>();
        services.AddSingleton<IExceptionProblemMapper, DomainExceptionProblemMapper>();
        services.AddSingleton<IExceptionProblemMapper, UniqueConstraintProblemMapper>();
        services.AddSingleton<IExceptionProblemMapper, FallbackProblemMapper>();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}