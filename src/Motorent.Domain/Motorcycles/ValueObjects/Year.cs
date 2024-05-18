using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Motorcycles.ValueObjects;

public sealed class Year : ValueObject
{
    internal static readonly Error ToolOld = Error.Validation(
        "O ano da moto não deve ser inferior a 5 anos.");
 
    private const int YearsOldThreshold = 5;
    
    private Year()
    {
    }
    
    public int Value { get; private init; }

    public static Result<Year> Create(int value)
    {
        return value >= DateTime.UtcNow.Year - YearsOldThreshold 
            ? new Year { Value = value } 
            : ToolOld;
    }
    
    public override string ToString() => Value.ToString();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}