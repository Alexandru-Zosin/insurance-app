using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Swagger;

public sealed class StringEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        var enumType = Nullable.GetUnderlyingType(type) ?? type;
        if (!enumType.IsEnum)
            return;

        schema.Type = "string";
        schema.Format = null;

        schema.Enum.Clear();

        foreach (var name in Enum.GetNames(enumType))
            schema.Enum.Add(new OpenApiString(name));
    }
}
