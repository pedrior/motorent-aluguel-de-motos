namespace Motorent.Domain.Motorcycles.ValueObjects;

public sealed class MotorcycleId(Ulid id) : EntityId<Ulid>(id)
{
    public static MotorcycleId New() => new(Ulid.NewUlid());
}