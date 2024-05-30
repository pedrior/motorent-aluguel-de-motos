using MassTransit;
using Microsoft.Extensions.Options;
using Motorent.Contracts.Common.Messages;
using Motorent.NotificationsWorker.Consumers;
using Motorent.NotificationsWorker.Persistence;
using Npgsql;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        builder.Sources.Clear();
        builder.AddConfiguration(configuration);
    })
    .ConfigureServices((_, services) =>
    {
        services.AddOptions<MessageBusOptions>()
            .Bind(configuration.GetSection(MessageBusOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<DataContext>(
            _ => new DataContext(configuration.GetConnectionString("DefaultConnection")!));

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumer<MotorcycleRegisteredConsumer>();

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
        });
    }).Build();

host.Run();