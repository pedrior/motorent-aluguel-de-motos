namespace Motorent.Contracts.Rentals.Requests;

public sealed record RentRequest
{
    public string Plan { get; init; } = null!;

    public string MotorcycleId { get; init; } = null!;
}