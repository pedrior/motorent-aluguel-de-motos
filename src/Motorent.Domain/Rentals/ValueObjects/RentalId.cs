namespace Motorent.Domain.Rentals.ValueObjects;

public sealed class RentalId(Ulid id) : EntityId<Ulid>(id)
{
    public static RentalId New() => new RentalId(Ulid.NewUlid());
}