using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Renters.Enums;

namespace Motorent.Domain.Renters.ValueObjects;

public sealed class CNH : ValueObject
{
    public static readonly Error Expired = Error.Validation("The CNH is expired.", code: "cnh");

    public static readonly Error Invalid = Error.Validation("Invalid CNH number.", code: "cnh");

    private const string CNHOnes = "11111111111";

    public string Number { get; private init; } = null!;

    public CNHCategory Category { get; private init; } = null!;

    public DateOnly ExpirationDate { get; private init; }

    public static Result<CNH> Create(string number, CNHCategory category, DateOnly expirationDate)
    {
        if (IsExpired(expirationDate))
        {
            return Expired;
        }

        if (IsCNHNumberInvalid(number))
        {
            return Invalid;
        }

        return new CNH
        {
            Number = number,
            Category = category,
            ExpirationDate = expirationDate
        };
    }

    public override string ToString() => $"{Number} - {Category} - {ExpirationDate}";

    private static bool IsExpired(DateOnly date) => date < DateOnly.FromDateTime(DateTime.UtcNow);

    private static bool IsCNHNumberInvalid(ReadOnlySpan<char> number)
    {
        if (number.Length is not 11 || number.SequenceEqual(CNHOnes))
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
        yield return ExpirationDate;
    }
}