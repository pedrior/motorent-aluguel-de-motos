using System.Data.Common;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Motorent.Infrastructure.Common.Persistence;
using Motorent.Infrastructure.Common.Persistence.Interceptors;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Motorent.Api.IntegrationTests.Common;

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
        builder.ConfigureServices(services =>
        {
            ConfigurePersistence(services, databaseContainer.GetConnectionString());
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