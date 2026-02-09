using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Swagger;

public sealed class StringEnumSchemaFilterForSwagger : ISchemaFilter
{
    /// <summary>
    /// OpenAPI/Swagger schema (the node being generated for a request/response contract
    /// type or one of its properties/parameters - recursive generation) filter that documents enums as strings 
    /// instead of integers.
    /// ---
    /// Example:
    ///  Default: { "type": "integer", "enum": [0, 1] }
    /// | After:   { "type": "string",  "enum": ["House", "Apartment"] }
    /// </summary>
    public void Apply(OpenApiSchema schemaBeingGenerated, SchemaFilterContext schemaGenerationContext)
    {
        Type typeForThisSchema = schemaGenerationContext.Type;
        // e.g.: BuildingType? -> BuildingType, so we can read enum's names
        Type enumTypeIfNullable = Nullable.GetUnderlyingType(typeForThisSchema) ?? typeForThisSchema;
        
        if (!enumTypeIfNullable.IsEnum)
            return;

        schemaBeingGenerated.Type = "string";
        schemaBeingGenerated.Format = null;

        schemaBeingGenerated.Enum.Clear();
        foreach (string allowedEnumNameInRequestOrResponseJson in Enum.GetNames(enumTypeIfNullable))
            schemaBeingGenerated.Enum.Add(new OpenApiString(allowedEnumNameInRequestOrResponseJson));
    }
}

