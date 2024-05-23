namespace Motorent.Contracts.Rentals.Requests;

public sealed record UpdateReturnDateRequest
{
    public DateOnly ReturnDate { get; init; }
}