using Coravel.Invocable;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Motorent.Domain.Common.Events;
using Motorent.Infrastructure.Common.Outbox;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Common.Jobs;

internal sealed class ProcessOutboxMessagesJob(
    DataContext dataContext,
    IPublisher publisher,
    ILogger<ProcessOutboxMessagesJob> logger) : IInvocable
{
    public async Task Invoke()
    {
        var messages = await FetchIncomingOutboxMessagesAsync();
        if (messages.Count is 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            logger.LogInformation("Processing outbox message {MessageId}. Attempt: {Attempt}",
                message.Id, message.Attempt);

            var @event = OutboxMessageSerializer.Deserialize<IEvent>(message.Data);
            if (@event is null)
            {
                logger.LogError("Failed to deserialize Outbox message ({@Message}) to type {Type}",
                    message, typeof(IEvent));

                continue;
            }

            try
            {
                await publisher.Publish(@event);
                message.MarkAsProcessed();

                logger.LogInformation("Outbox message {MessageId} has been successfully processed", message.Id);
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(
                    errorType: ex.GetType().Name,
                    errorMessage: ex.Message,
                    errorDetails: ex.ToString());

                logger.LogError(ex, "Failed to process outbox message ({@Message})", message);
            }
        }

        await dataContext.SaveChangesAsync();
    }

    private Task<List<OutboxMessage>> FetchIncomingOutboxMessagesAsync()
    {
        return dataContext.OutboxMessages
            .Where(om => om.Status == OutboxMessageStatus.Pending
                         || (om.Status == OutboxMessageStatus.Retry && om.NextAttemptAt != null
                                                                    && om.NextAttemptAt <= DateTime.UtcNow))
            .OrderBy(om => om.CreatedAt)
            .Take(20)
            .ToListAsync();
    }
}