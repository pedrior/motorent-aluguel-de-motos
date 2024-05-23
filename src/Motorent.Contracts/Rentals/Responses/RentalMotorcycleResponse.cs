namespace Motorent.Contracts.Rentals.Responses;

public sealed record RentalMotorcycleResponse
{
    public string Id { get; init; } = null!;
    
    public string Model { get; init; } = null!;
    
    public int Year { get; init; }
    
    public string LicensePlate { get; init; } = null!;
}