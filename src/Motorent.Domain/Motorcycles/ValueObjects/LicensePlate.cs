using System.Text.RegularExpressions;
using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Motorcycles.ValueObjects;

public sealed partial class LicensePlate : ValueObject
{
    public static readonly Error Malformed = Error.Validation(
        "The license plate provided is invalid. Must be in the format AAA0A00.", 
        code: "license_plate");
    
    private LicensePlate()
    {
    }

    public string Value { get; private init; } = null!;

    public static Result<LicensePlate> Create(string value)
    {
        value = value.ToUpperInvariant();
        return IsValid(value)
            ? new LicensePlate { Value = value }
            : Malformed;
    }

    private static bool IsValid(string value) => LicensePlateRegex().IsMatch(value);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("^[A-Z]{3}[0-9][A-Z][0-9]{2}$", RegexOptions.Compiled)]
    private static partial Regex LicensePlateRegex();
}