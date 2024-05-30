using MassTransit;
using Motorent.Contracts.Common.Messages;
using Motorent.NotificationsWorker.Persistence;
using Motorent.NotificationsWorker.Persistence.Models;
using Newtonsoft.Json;

namespace Motorent.NotificationsWorker.Consumers;

internal sealed class MotorcycleRegisteredConsumer(
    DataContext dataContext,
    ILogger<MotorcycleRegisteredConsumer> logger)
    : IConsumer<MotorcycleRegisteredMessage>
{
    public async Task Consume(ConsumeContext<MotorcycleRegisteredMessage> context)
    {
        var message = context.Message;

        logger.LogInformation("Handling message {MessageName} ({@Message})",
            nameof(MotorcycleRegisteredMessage), message);

        await SaveMessageAsync(message);

        NotifyNewMotorcycleRegistered(message);
    }

    private static bool IsNewMotorcycle(int year) => year == DateTimeOffset.UtcNow.Year;

    private void NotifyNewMotorcycleRegistered(MotorcycleRegisteredMessage message)
    {
        if (!IsNewMotorcycle(message.Year))
        {
            return;
        }

        // TODO: Implementar notificação de novo veículo registrado por e-mail depois
        logger.LogInformation("New motorcycle registered: {Id} {Model} {Year}",
            message.MotorcycleId.ToString(), message.Model, message.Year);
    }

    private async Task SaveMessageAsync(MotorcycleRegisteredMessage message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Message = JsonConvert.SerializeObject(message),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await dataContext.EnsureInitializedAsync();
        using var connection = dataContext.CreateConnection();
        await connection.ExecuteAsync(
            $"""
             INSERT INTO {TableNames.Notifications} (id, message, created_at)
             VALUES (@Id, @Message, @CreatedAt);
             """,
            notification);
    }
}