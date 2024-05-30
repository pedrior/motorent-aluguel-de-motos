using System.Data.Common;
using FakeItEasy;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Infrastructure.Common.Jobs;
using Motorent.Infrastructure.Common.Persistence;
using Motorent.Infrastructure.Common.Persistence.Interceptors;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Motorent.Api.IntegrationTests.TestUtils;

public sealed class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("motorent")
        .Build();

    private Respawner respawner = null!;
    private DbConnection dbConnection = null!;

    public Task ResetDatabaseAsync() => respawner.ResetAsync(dbConnection);

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();

        await MigrateDatabaseAsync();
        await InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        await dbConnection.DisposeAsync();
        await dbContainer.DisposeAsync();
    }

    private async Task MigrateDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DataContext>()
            .Database.MigrateAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        dbConnection = new NpgsqlConnection(dbContainer.GetConnectionString());
        await dbConnection.OpenAsync();

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

        builder.ConfigureTestServices(services =>
        {
            ConfigureTestPersistenceServices(services);
            ConfigureTestBackgroundJobsServices(services);
            ConfigureTestStorageServices(services);
            ConfigureTestMessagingServices(services);
        });
    }

    private static void OverrideConfigurations(IWebHostBuilder builder)
    {
        var data = new Dictionary<string, string?>
        {
            { "AWS:Region", "us-east-1" },
            { "AWS:Profile", "some-profile" },
            { "Storage:BucketName", "some-bucket" }
        };

        builder.UseConfiguration(new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build());
    }

    private void ConfigureTestPersistenceServices(IServiceCollection services)
    {
        services.RemoveAll<NpgsqlDataSource>();
        services.RemoveAll<NpgsqlConnection>();
        services.RemoveAll<DbContextOptions<DataContext>>();

        services.AddNpgsqlDataSource(dbContainer.GetConnectionString(),
            npgsqlBuilder => npgsqlBuilder.EnableDynamicJson());

        services.AddDbContext<DataContext>((provider, options) =>
        {
            options.EnableServiceProviderCaching(false);

            options.UseNpgsql(provider.GetRequiredService<NpgsqlDataSource>(), npgsqlOptions =>
                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            options.AddInterceptors(
                provider.GetRequiredService<AuditEntitiesOnSaveChangesInterceptor>(),
                provider.GetRequiredService<PersistOutboxDomainEventsOnSaveChangesInterceptor>());
        });
    }

    private static void ConfigureTestBackgroundJobsServices(IServiceCollection services)
    {
        services.RemoveAll<GlobalConfiguration>();
        services.RemoveAll<IProcessOutboxMessagesJob>();

        services.AddHangfire(config => config.UseInMemoryStorage());
    }

    private static void ConfigureTestStorageServices(IServiceCollection services)
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

    private static void ConfigureTestMessagingServices(IServiceCollection services)
    {
        services.RemoveMassTransitHostedService();
        services.AddMassTransitTestHarness();
    }
}