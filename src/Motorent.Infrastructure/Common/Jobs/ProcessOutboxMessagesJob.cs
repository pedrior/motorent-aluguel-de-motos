using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Motorent.Domain.Common;
using Motorent.Infrastructure.Common.Outbox;
using Motorent.Infrastructure.Common.Persistence;
using Newtonsoft.Json;

namespace Motorent.Infrastructure.Common.Jobs;

internal sealed class ProcessOutboxMessagesJob(
    DataContext dataContext,
    IPublisher publisher,
    ILogger<ProcessOutboxMessagesJob> logger) : IProcessOutboxMessagesJob
{
    public async Task ProcessAsync()
    {
        logger.LogInformation("Processing Outbox messages...");

        var messages = await FetchIncomingOutboxMessagesAsync();
        if (messages.Count is 0)
        {
            logger.LogInformation("No Outbox messages to process");
            return;
        }

        await using var transaction = await dataContext.Database.BeginTransactionAsync();

        foreach (var message in messages)
        {
            try
            {
                var @event = JsonConvert.DeserializeObject<IEvent>(
                    message.Data, OutboxMessage.JsonSerializerSettings)!;
                
                await publisher.Publish(@event);

                message.MarkAsProcessed();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error processing Outbox message with Id: {Id}", message.Id);
                
                message.MarkAsFailed(exception.ToString());
            }
            
            await dataContext.SaveChangesAsync();
        }
        
        await transaction.CommitAsync();
        
        logger.LogInformation("Processed {Count} Outbox messages", messages.Count);
    }

    private Task<List<OutboxMessage>> FetchIncomingOutboxMessagesAsync()
    {
        return dataContext.OutboxMessages
            .Where(om => om.ProcessedAt == null)
            .OrderBy(om => om.CreatedAt)
            .Take(20)
            .ToListAsync();
    }
}