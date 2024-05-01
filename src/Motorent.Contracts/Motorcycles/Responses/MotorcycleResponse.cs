namespace Motorent.Contracts.Motorcycles.Responses;

public sealed record MotorcycleResponse
{
    public Ulid Id { get; init; }

    public string Model { get; init; } = null!;

    public string Brand { get; init; } = null!;

    public int Year { get; init; }

    public decimal DailyPrice { get; init; }

    public string LicensePlate { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}