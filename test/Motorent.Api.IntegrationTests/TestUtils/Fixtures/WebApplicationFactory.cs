using System.Data.Common;
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
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("motorent")
        .WithUsername("root")
        .WithPassword("password")
        .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilCommandIsCompleted("pg_isready"))
        .Build();

    private Respawner respawner = null!;
    private DbConnection dbConnection = null!;
    
    public Task ResetDatabaseAsync() => respawner.ResetAsync(dbConnection);

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();
        
        await MigrateDatabaseAsync();
        await OpenDatabaseConnection();
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await dbConnection.CloseAsync();
        await dbContainer.StopAsync();
    }
    
    private async Task OpenDatabaseConnection()
    {
        dbConnection = new NpgsqlConnection(dbContainer.GetConnectionString());
        await dbConnection.OpenAsync();
    }

    private async Task MigrateDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DataContext>()
            .Database.MigrateAsync();
    }
    
    private async Task InitializeRespawnerAsync()
    {
        respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        OverrideConfigurations(builder);

        builder.ConfigureServices(services =>
        {
            AddPersistence(services);
            AddBackgroundJobs(services);
            AddStorage(services);
        });
    }

    private static void OverrideConfigurations(IWebHostBuilder builder)
    {
        var config = new Dictionary<string, string?>
        {
            { "AWS:Region", "us-east-1" },
            { "AWS:Profile", "some-profile" },
            { "Storage:BucketName", "some-bucket" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();

        builder.UseConfiguration(configuration);
    }

    private void AddPersistence(IServiceCollection services)
    {
        services.RemoveAll<NpgsqlDataSource>();
        services.RemoveAll<NpgsqlConnection>();
        services.RemoveAll<DbContextOptions<DataContext>>();

        services.AddNpgsqlDataSource(dbContainer.GetConnectionString(), builder => builder.EnableDynamicJson());

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

    private static void AddBackgroundJobs(IServiceCollection services)
    {
        services.RemoveAll(typeof(GlobalConfiguration));

        services.AddHangfire(config => config.UseInMemoryStorage());
    }

    private static void AddStorage(IServiceCollection services)
    {
        services.RemoveAll<IStorageService>();

        services.AddTransient<IStorageService>(_ =>
        {
            var fake = A.Fake<IStorageService>();

            A.CallTo(() => fake.GenerateUrlAsync(A<Uri>._, A<int>._))
                .Returns(new Uri("https://example.com/image.png"));

            return fake;
        });
    }
}