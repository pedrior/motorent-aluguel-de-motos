namespace Motorent.Contracts.Rentals.Requests;

public sealed record ListRentalsRequest
{
    public int Page { get; init; } = 1;

    public int Limit { get; init; } = 10;
}