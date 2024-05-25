using System.Text;
using Amazon.S3;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Messaging;
using Motorent.Application.Common.Abstractions.Persistence;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Jobs;
using Motorent.Infrastructure.Common.Messaging;
using Motorent.Infrastructure.Common.Persistence;
using Motorent.Infrastructure.Common.Persistence.Interceptors;
using Motorent.Infrastructure.Common.Security;
using Motorent.Infrastructure.Common.Storage;
using Motorent.Infrastructure.Motorcycles;
using Motorent.Infrastructure.Motorcycles.Consumers;
using Motorent.Infrastructure.Motorcycles.Persistence;
using Motorent.Infrastructure.Rentals.Persistence;
using Motorent.Infrastructure.Renters;
using Motorent.Infrastructure.Renters.Persistence;
using Npgsql;
using Serilog;

namespace Motorent.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSerilog(config => config
            .ReadFrom.Configuration(configuration));

        services.AddAuthentication(configuration);

        services.AddAuthorization();

        services.AddPersistence(configuration);

        services.AddBackgroundJobs(configuration);

        services.AddStorage(configuration);

        services.AddMessaging(configuration);
        
        services.AddHttpContextAccessor();

        services.AddTransient<TimeProvider>(_ => TimeProvider.System);

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ILicensePlateService, LicensePlateService>();
        services.AddScoped<IMotorcycleDeletionService, MotorcycleDeletionService>();

        services.AddScoped<IRentalFactory, RentalFactory>();
        services.AddScoped<IRentalPenaltyService, RentalPenaltyService>();
        
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IDriverLicenseService, DriverLicenseService>();

        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<AuditEntitiesOnSaveChangesInterceptor>();
        services.AddScoped<PersistOutboxDomainEventsOnSaveChangesInterceptor>();

        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IRenterRepository, RenterRepository>();

        services.AddNpgsqlDataSource(configuration.GetConnectionString("DefaultConnection")!,
            builder => builder.EnableDynamicJson());

        services.AddDbContext<DataContext>((provider, options) =>
        {
            options.UseNpgsql(provider.GetRequiredService<NpgsqlDataSource>(), pgsqlOptions =>
                pgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            options.AddInterceptors(
                provider.GetRequiredService<AuditEntitiesOnSaveChangesInterceptor>(),
                provider.GetRequiredService<PersistOutboxDomainEventsOnSaveChangesInterceptor>());
        });
    }

    private static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(hangfireConfiguration =>
        {
            hangfireConfiguration.UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")!));
        });

        services.AddHangfireServer(options => { options.SchedulePollingInterval = TimeSpan.FromSeconds(1); });

        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();
    }

    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddTransient<ISecurityTokenProvider, SecurityTokenProvider>();

        var securityTokenOptionsSection = configuration.GetSection(SecurityTokenOptions.SectionName);
        services.AddOptions<SecurityTokenOptions>()
            .Bind(securityTokenOptionsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var securityTokenOptions = securityTokenOptionsSection.Get<SecurityTokenOptions>()!;
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = securityTokenOptions.Issuer,
                    ValidAudience = securityTokenOptions.Audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(securityTokenOptions.Key))
                };
            });
    }
    
    private static void AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        
        services.AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddAWSService<IAmazonS3>();
        
        services.AddScoped<IStorageService, StorageService>();
    }

    private static void AddMessaging(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddOptions<MessageBusOptions>()
            .Bind(configuration.GetSection(MessageBusOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        service.AddTransient<IMessageBus, MessageBus>();

        service.AddReceiveObserver<MessageLogging>();
        
        service.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            
            config.UsingRabbitMq((context, configurator) =>
            {
                var options = context.GetRequiredService<IOptions<MessageBusOptions>>().Value;
                configurator.Host(options.Host, hostConfigurator =>
                {
                    hostConfigurator.Username(options.Username);
                    hostConfigurator.Password(options.Password);
                });
                
                configurator.ConfigureEndpoints(context);
            });
            
            config.AddConsumer<MotorcycleRegisteredConsumer>();
        });
    }
}