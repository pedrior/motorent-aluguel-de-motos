using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Rentals.ValueObjects;

public sealed class Money : ValueObject
{
    public static readonly Money Zero = new(0m);

    private Money()
    {
    }
    
    public Money(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        Value = value;
    }

    public decimal Value { get; }

    public static Money operator +(Money left, Money right) => new(left.Value + right.Value);

    public static Money operator -(Money left, Money right) => new(left.Value - right.Value);

    public static Money operator *(Money left, Money right) => new(left.Value * right.Value);
    
    public static Money operator *(Money left, int right) => new(left.Value * right);
    
    public static Money operator *(Money left, decimal right) => new(left.Value * right);

    public override string ToString() => Value.ToString("C");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}