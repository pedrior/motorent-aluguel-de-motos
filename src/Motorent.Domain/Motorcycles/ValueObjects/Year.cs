using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Motorcycles.ValueObjects;

public sealed class Year : ValueObject
{
    public static readonly Error YearTooOld = Error.Validation(
        "The year provided is too old. Must be at most 5 years old.", code: "year");
 
    private const int YearsOldThreshold = 5;
    
    private Year()
    {
    }
    
    public int Value { get; private init; }

    public static Result<Year> Create(int value)
    {
        return value >= DateTime.UtcNow.Year - YearsOldThreshold 
            ? new Year { Value = value } 
            : YearTooOld;
    }
    
    public override string ToString() => Value.ToString();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}