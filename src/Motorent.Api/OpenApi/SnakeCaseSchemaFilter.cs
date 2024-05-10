using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Motorent.Api.OpenApi;

internal sealed class SnakeCaseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties is null)
        {
            return;
        }

        if (schema.Properties.Count is 0)
        {
            return;
        }
        
        var properties = new Dictionary<string, OpenApiSchema>();
        foreach (var key in schema.Properties.Keys)
        {
            properties[ToSnakeCase(key)] = schema.Properties[key];
        }

        schema.Properties = properties;
    }
    
    private static string ToSnakeCase(string str)
    {
        return string.Concat(str.Select((c, index) => index > 0 && char.IsUpper(c)
                ? "_" + c
                : c.ToString()))
            .ToLower();
    }
}