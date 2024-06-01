namespace Motorent.Domain.Rentals.ValueObjects;

public sealed class RentalId(Ulid id) : EntityId<Ulid>(id)
{
    public RentalId() : this(Ulid.Empty)
    {
    }

    public static RentalId New() => new(Ulid.NewUlid());
}