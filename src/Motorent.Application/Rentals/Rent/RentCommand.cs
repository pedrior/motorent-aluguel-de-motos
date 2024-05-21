using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Rentals.Responses;

namespace Motorent.Application.Rentals.Rent;

public sealed record RentCommand : ICommand<RentalResponse>
{
    public required string Plan { get; init; }
    
    public required Ulid MotorcycleId { get; init; }
}