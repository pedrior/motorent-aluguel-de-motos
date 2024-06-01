using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.Rentals.Events;

public sealed class RentalCreated(RentalId rentalId) : IEvent
{
    public RentalId RentalId { get; } = rentalId;
}