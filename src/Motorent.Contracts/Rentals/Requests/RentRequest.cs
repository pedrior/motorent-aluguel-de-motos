namespace Motorent.Contracts.Rentals.Requests;

public sealed record RentRequest
{
    public string Plan { get; init; } = null!;

    public Ulid MotorcycleId { get; init; }
}