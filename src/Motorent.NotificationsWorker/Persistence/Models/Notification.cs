namespace Motorent.NotificationsWorker.Persistence.Models;

internal sealed class Notification
{
    public required Guid Id { get; set; }
    
    public required string Message { get; set; }
    
    public required DateTimeOffset CreatedAt { get; set; }
}