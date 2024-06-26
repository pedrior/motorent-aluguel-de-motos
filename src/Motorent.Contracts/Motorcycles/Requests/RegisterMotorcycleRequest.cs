namespace Motorent.Contracts.Motorcycles.Requests;

public sealed record RegisterMotorcycleRequest
{
    public string Model { get; init; } = null!;

    public int Year { get; init; }
    
    public string LicensePlate { get; init; } = null!;
}