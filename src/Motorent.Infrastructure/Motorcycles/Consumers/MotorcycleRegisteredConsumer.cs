using MassTransit;
using Microsoft.Extensions.Logging;
using Motorent.Application.Motorcycles.Common.Messaging;

namespace Motorent.Infrastructure.Motorcycles.Consumers;

internal sealed class MotorcycleRegisteredConsumer(ILogger<MotorcycleRegisteredConsumer> logger) 
    : IConsumer<MotorcycleRegisteredMessage>
{
    public Task Consume(ConsumeContext<MotorcycleRegisteredMessage> context)
    {
        var message = context.Message;
        
        logger.LogInformation("Handling message {MessageName} ({@Message})",
            nameof(MotorcycleRegisteredMessage), message);

        if (IsNewMotorcycle(message.Year))
        {
            // TODO: Implementar notificação de novo veículo registrado por e-mail depois
            
            logger.LogInformation("New motorcycle registered: {Id} {Model} {Year}",
                message.MotorcycleId.ToString(), message.Model, message.Year);
        }
        
        return Task.CompletedTask;
    }
    
    private static bool IsNewMotorcycle(int year) => year == DateTimeOffset.UtcNow.Year;
}