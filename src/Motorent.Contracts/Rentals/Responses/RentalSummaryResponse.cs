namespace Motorent.Contracts.Rentals.Responses;

public sealed record RentalSummaryResponse
{
    public string Id { get; init; } = null!;

    public string Plan { get; init; } = null!;
    
    public bool IsActive { get; init; }
    
    public decimal DailyPrice { get; init; }
    
    public DateOnly Start { get; init; }
    
    public DateOnly Return { get; init; }
    
    public DateOnly End { get; init; }
    
    public RentalMotorcycleResponse Motorcycle { get; init; } = null!;
}