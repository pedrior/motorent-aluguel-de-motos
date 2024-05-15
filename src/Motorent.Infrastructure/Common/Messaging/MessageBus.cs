using MassTransit;
using Motorent.Application.Common.Abstractions.Messaging;

namespace Motorent.Infrastructure.Common.Messaging;

internal sealed class MessageBus(IPublishEndpoint publishEndpoint) : IMessageBus
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class =>
        publishEndpoint.Publish(message, cancellationToken);
}