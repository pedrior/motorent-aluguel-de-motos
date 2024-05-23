namespace Motorent.Contracts.Motorcycles.Responses;

public sealed record MotorcycleSummaryResponse
{
    public string Id { get; init; } = null!;

    public string Model { get; init; } = null!;

    public int Year { get; init; }

    public string LicensePlate { get; init; } = null!;
}