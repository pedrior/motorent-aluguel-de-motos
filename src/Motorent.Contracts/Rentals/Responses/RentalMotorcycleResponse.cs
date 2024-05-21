namespace Motorent.Contracts.Rentals.Responses;

public sealed record RentalMotorcycleResponse
{
    public Ulid Id { get; init; }
    
    public string Model { get; init; } = null!;
    
    public int Year { get; init; }
    
    public string LicensePlate { get; init; } = null!;
}