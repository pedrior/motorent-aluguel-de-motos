namespace Motorent.Infrastructure.Common.Messaging;

internal sealed class MessageLog
{
    public required Ulid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required string Identifier { get; init; }
    
    public required string Data { get; init; }
    
    public required DateTimeOffset? SentAt { get; init; }
    
    public required DateTimeOffset ReceivedAt { get; init; }
}