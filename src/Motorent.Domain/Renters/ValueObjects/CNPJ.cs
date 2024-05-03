using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Renters.ValueObjects;

public sealed class CNPJ : ValueObject
{
    public static readonly Error Invalid = Error.Validation("The CNPJ is invalid.", code: "cnpj");

    private const int Length = 14;
    private const int MaxLength = 18;

    private static readonly int[] Multipliers1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] Multipliers2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    private CNPJ()
    {
    }

    public string Value { get; private init; } = null!;

    public static Result<CNPJ> Create(string value)
    {
        return IsCNPJInvalid(value) 
            ? Invalid 
            : new CNPJ { Value = value };
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static bool IsCNPJInvalid(string value)
    {
        if (value.Length is < Length or > MaxLength)
        {
            return true;
        }

        var index = 0;
        Span<char> number = stackalloc char[Length];
        foreach (var digit in value.Where(char.IsDigit))
        {
            number[index++] = digit;
            if (index is Length)
            {
                break;
            }
        }

        if (number.Length is not Length)
        {
            return true;
        }

        var sum1 = 0;
        for (var i = 0; i < 12; i++)
        {
            sum1 += (number[i] - '0') * Multipliers1[i];
        }

        var rem1 = sum1 % 11;
        var vd1 = rem1 < 2 ? 0 : 11 - rem1;

        var sum2 = 0;
        for (var i = 0; i < 13; i++)
        {
            sum2 += (number[i] - '0') * Multipliers2[i];
        }

        var rem2 = sum2 % 11;
        var vd2 = rem2 < 2 ? 0 : 11 - rem2;

        return vd1 != number[12] - '0' || vd2 != number[13] - '0';
    }
}