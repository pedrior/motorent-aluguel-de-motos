namespace Motorent.Domain.Common.ValueObjects;

public sealed class Money : ValueObject
{
    public static readonly Error CannotBeNegative = Error.Validation(
        "Money cannot be negative.", code: "money");

    private Money()
    {
    }

    public decimal Value { get; private init; }

    public static Result<Money> Create(decimal value)
    {
        if (value < 0m)
        {
            return CannotBeNegative;
        }

        return new Money { Value = value };
    }

    public override string ToString() => Value.ToString("C");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}