using System.Data.Common;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using DotNet.Testcontainers.Builders;
using FakeItEasy;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Infrastructure.Common.Persistence;
using Motorent.Infrastructure.Common.Persistence.Interceptors;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Motorent.Api.IntegrationTests.TestUtils.Fixtures;

public sealed class WebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer databaseContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("motorent")
        .WithUsername("root")
        .WithPassword("password")
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilCommandIsCompleted("pg_isready"))
        .Build();

    private Respawner respawner = null!;
    private DbConnection connection = null!;

    internal DataContext DataContext { get; private set; } = null!;

    public Task ResetDatabaseAsync()
    {
        DataContext.ChangeTracker.Clear();
        
        return respawner.ResetAsync(connection);
    }

    public async Task InitializeAsync()
    {
        await databaseContainer.StartAsync();

        await InitializeDatabaseAsync();
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await connection.CloseAsync();
        await databaseContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configurationValues = new Dictionary<string, string?>
        {
            { "AWS:Region", "us-east-1" },
            { "AWS:Profile", "some-profile" },
            { "Storage:BucketName", "some-bucket" }
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        // Used during the creation of the application
        builder.UseConfiguration(configuration);
        
        builder.ConfigureServices(services =>
        {
            ConfigurePersistence(services, databaseContainer.GetConnectionString());
            ConfigureBackgroundJobs(services);
            ConfigureStorage(services);
        });
    }

    private static void ConfigurePersistence(IServiceCollection services, string connectionString)
    {
        services.RemoveAll<NpgsqlDataSource>();
        services.RemoveAll<NpgsqlConnection>();
        services.RemoveAll<DbContextOptions<DataContext>>();
        
        services.AddNpgsqlDataSource(connectionString, builder => builder.EnableDynamicJson());
        
        services.AddDbContext<DataContext>((provider, options) =>
        {
            options.UseNpgsql(provider.GetRequiredService<NpgsqlDataSource>(), pgsqlOptions =>
                pgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            options.AddInterceptors(
                provider.GetRequiredService<AuditEntitiesOnSaveChangesInterceptor>(),
                provider.GetRequiredService<PersistOutboxDomainEventsOnSaveChangesInterceptor>());
            
            options.EnableServiceProviderCaching(false);
        });
    }
    
    private static void ConfigureBackgroundJobs(IServiceCollection services)
    {
        services.RemoveAll(typeof(GlobalConfiguration));
        
        services.AddHangfire(config => config.UseInMemoryStorage());
    }
    
    private static void ConfigureStorage(IServiceCollection services)
    {
        services.RemoveAll<IStorageService>();
        
        services.AddSingleton<IStorageService>(_ =>
        {
           var storageService = A.Fake<IStorageService>();

           A.CallTo(() => storageService.GenerateUrlAsync(A<Uri>._, A<int>._))
               .Returns(new Uri($"https://bucket-name.s3.region.amazonaws.com/{Ulid.NewUlid()}.png"));
           
           return storageService;
        });
    }
    
    private async Task InitializeDatabaseAsync()
    {
        DataContext = Services.CreateScope()
            .ServiceProvider.GetRequiredService<DataContext>();

        await DataContext.Database.EnsureDeletedAsync();
        await DataContext.Database.MigrateAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        connection = new NpgsqlConnection(databaseContainer.GetConnectionString());

        await connection.OpenAsync();

        respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }
}