namespace Motorent.Domain.Common;

public abstract class EntityId<T>(T id) : ValueObject where T : notnull
{
    public T Value { get; init; } = id;

    public override string ToString() => Value.ToString()!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}