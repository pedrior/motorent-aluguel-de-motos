using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Motorent.Infrastructure.Common.Persistence;
using Newtonsoft.Json;

namespace Motorent.Infrastructure.Common.Messaging;

internal sealed class MessageLogging(IServiceScopeFactory serviceScopeFactory) : IReceiveObserver
{
    public Task PreReceive(ReceiveContext context) => Task.CompletedTask;

    public Task PostReceive(ReceiveContext context) => Task.CompletedTask;

    public async Task PostConsume<T>(
        ConsumeContext<T> context,
        TimeSpan duration,
        string consumerType) where T : class
    {
        var messageLog = new MessageLog
        {
            Id = Ulid.NewUlid(),
            Name = context.Message.GetType().Name,
            Identifier = context.MessageId?.ToString() ?? string.Empty,
            Data = JsonConvert.SerializeObject(context.Message),
            SentAt = context.SentTime,
            ReceivedAt = DateTimeOffset.UtcNow
        };

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        await dataContext.MessageLogs.AddAsync(messageLog);
        await dataContext.SaveChangesAsync();
    }

    public Task ConsumeFault<T>(
        ConsumeContext<T> context,
        TimeSpan duration,
        string consumerType,
        Exception exception) where T : class
    {
        return Task.CompletedTask;
    }

    public Task ReceiveFault(ReceiveContext context, Exception exception) => Task.CompletedTask;
}