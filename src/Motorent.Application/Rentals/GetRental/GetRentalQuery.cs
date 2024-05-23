using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Rentals.Responses;

namespace Motorent.Application.Rentals.GetRental;

public sealed record GetRentalQuery : IQuery<RentalResponse>
{
    public required Ulid Id { get; init; }
}