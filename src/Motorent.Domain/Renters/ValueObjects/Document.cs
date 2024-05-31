namespace Motorent.Domain.Renters.ValueObjects;

public sealed class Document : ValueObject
{
    internal static readonly Error Invalid = Error.Validation("Documento CNPJ inválido.");

    private static readonly int[] Multipliers1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] Multipliers2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    private Document()
    {
    }

    public string Value { get; private init; } = null!;

    public static Result<Document> Create(string value)
    {
        value = Sanitize(value);
        return Validate(value) ? new Document { Value = value } : Invalid;
    }

    public override string ToString() => $"{Value[..2]}.{Value[2..5]}.{Value[5..8]}/{Value[8..12]}-{Value[12..]}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static string Sanitize(string value)
    {
        var index = 0;
        Span<char> sanitized = stackalloc char[14];
        foreach (var digit in value.Where(char.IsDigit))
        {
            sanitized[index++] = digit;
            if (index is 14)
            {
                break;
            }
        }

        return new string(sanitized);
    }
    
    private static bool Validate(string value)
    {
        if (value.Length is not 14)
        {
            return false;
        }

        var sum1 = 0;
        for (var i = 0; i < 12; i++)
        {
            sum1 += (value[i] - '0') * Multipliers1[i];
        }

        var rem1 = sum1 % 11;
        var vd1 = rem1 < 2 ? 0 : 11 - rem1;

        var sum2 = 0;
        for (var i = 0; i < 13; i++)
        {
            sum2 += (value[i] - '0') * Multipliers2[i];
        }

        var rem2 = sum2 % 11;
        var vd2 = rem2 < 2 ? 0 : 11 - rem2;

        return vd1 == value[12] - '0' && vd2 == value[13] - '0';
    }
}