using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Renters.ValueObjects;

public sealed class Birthdate : ValueObject
{
    public static readonly Error MustBeAtLeast18YearsOld = Error.Validation(
        "Must be at least 18 years old.", code: "birthdate");

    private Birthdate()
    {
    }

    public DateOnly Value { get; private init; }

    public static Result<Birthdate> Create(DateOnly value)
    {
        return IsNot18YearsOld(value)
            ? MustBeAtLeast18YearsOld
            : new Birthdate { Value = value };
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");

    private static bool IsNot18YearsOld(DateOnly value)
    {
        var now = DateTime.Today;
        var age = now.Year - value.Year;

        if (now.Month < value.Month || (now.Month == value.Month && now.Day < value.Day))
        {
            age--;
        }

        return age < 18;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}