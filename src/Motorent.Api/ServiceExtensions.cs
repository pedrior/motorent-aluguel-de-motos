using Microsoft.OpenApi.Models;

namespace Motorent.Api;

internal static class ServiceExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwagger();
        
        return services;
    }
    
    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Motorent API",
                Version = "v1",
                Description = "Uma API REST para gerenciar aluguel de motos.",
                Contact = new OpenApiContact
                {
                    Name = "Pedro Júnior",
                    Email = "pedrojdev@gmail.com",
                    Url = new Uri("https://github.com/pedrior/motorent-aluguel-de-motos")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://github.com/pedrior/motorent-aluguel-de-motos/blob/master/LICENSE")
                }
            });

            setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    []
                }
            });
        });
    }
}