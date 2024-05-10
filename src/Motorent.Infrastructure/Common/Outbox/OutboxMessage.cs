using Newtonsoft.Json;

namespace Motorent.Infrastructure.Common.Outbox;

internal sealed class OutboxMessage
{
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    
    private OutboxMessage()
    {
    }

    public Guid Id { get; init; }

    public string Type { get; init; } = null!;

    public string Data { get; init; } = null!;
    
    public string? Error { get; private set; }

    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? ProcessedAt { get; private set; }

    public static OutboxMessage Create<T>(T entity) where T : class => new()
    {
        Id = Guid.NewGuid(),
        Type = entity.GetType().Name,
        Data = JsonConvert.SerializeObject(entity, JsonSerializerSettings),
        CreatedAt = DateTimeOffset.UtcNow
    };

    public void MarkAsProcessed() => ProcessedAt = DateTimeOffset.UtcNow;
    
    public void MarkAsFailed(string error)
    {
        Error = error;
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}