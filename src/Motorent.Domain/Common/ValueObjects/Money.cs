namespace Motorent.Domain.Common.ValueObjects;

public sealed class Money : ValueObject
{
    internal static readonly Error CannotBeNegative = Error.Validation(
        "O valor monetário não pode ser negativo.");

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