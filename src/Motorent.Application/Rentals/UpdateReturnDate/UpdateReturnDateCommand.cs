using Motorent.Application.Common.Abstractions.Requests;

namespace Motorent.Application.Rentals.UpdateReturnDate;

public sealed record UpdateReturnDateCommand : ICommand
{
    public required Ulid RentalId { get; init; }
    
    public required DateOnly ReturnDate { get; init; }
}