namespace Motorent.Domain.Common.Entities;

public interface IAuditable
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}