namespace Motorent.Contracts.Motorcycles.Responses;

public sealed record MotorcycleSummaryResponse
{
    public Ulid Id { get; init; }

    public string Model { get; init; } = null!;

    public int Year { get; init; }

    public string LicensePlate { get; init; } = null!;
}