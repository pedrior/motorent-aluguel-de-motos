using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Renters.Enums;

namespace Motorent.Domain.Renters.ValueObjects;

public sealed class DriverLicense : ValueObject
{
    internal static readonly Error Expired = Error.Validation("A CNH está expirada.");

    internal static readonly Error Invalid = Error.Validation("Número de CNH é inválido.");

    private const string Ones = "11111111111";

    private DriverLicense()
    {
    }

    public string Number { get; private init; } = null!;

    public DriverLicenseCategory Category { get; private init; } = null!;

    public DateOnly Expiry { get; private init; }

    public static Result<DriverLicense> Create(string number, DriverLicenseCategory category, DateOnly expiry)
    {
        if (IsExpired(expiry))
        {
            return Expired;
        }

        if (IsNumberInvalid(number))
        {
            return Invalid;
        }

        return new DriverLicense
        {
            Number = number,
            Category = category,
            Expiry = expiry
        };
    }

    public override string ToString() => $"{Number} - {Category} - {Expiry}";

    private static bool IsExpired(DateOnly date) => date < DateOnly.FromDateTime(DateTime.UtcNow);

    private static bool IsNumberInvalid(ReadOnlySpan<char> number)
    {
        if (number.Length is not 11 || number.SequenceEqual(Ones))
        {
            return true;
        }

        var sum1 = 0;
        for (int i = 0, j = 9; i < 9; i++, j--)
        {
            sum1 += (number[i] - '0') * j;
        }

        var rem1 = sum1 % 11;
        var vd1 = rem1 >= 10 ? 0 : rem1;

        var sum2 = 0;
        for (int i = 0, j = 1; i < 9; i++, j++)
        {
            sum2 += (number[i] - '0') * j;
        }

        var rem2 = sum2 % 11;
        var vd2 = rem2 >= 10 ? 0 : rem2;

        return vd1 * 10 + vd2 != int.Parse(number.Slice(9, 2));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Category;
        yield return Number;
        yield return Expiry;
    }
}