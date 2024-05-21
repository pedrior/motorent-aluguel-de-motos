namespace Motorent.Contracts.Rentals.Responses;

public sealed record RentalResponse
{
    public Ulid Id { get; init; }

    public string Plan { get; init; } = null!;
    
    public bool IsActive { get; init; }
    
    public decimal DailyPrice { get; init; }
    
    public decimal TotalPrice { get; init; }
    
    public DateOnly Start { get; init; }
    
    public DateOnly End { get; init; }
    
    public DateOnly Return { get; init; }

    public RentalMotorcycleResponse Motorcycle { get; init; } = null!;
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? UpdatedAt { get; init; }
}