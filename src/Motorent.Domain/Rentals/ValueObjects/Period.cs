using Motorent.Domain.Common.ValueObjects;

namespace Motorent.Domain.Rentals.ValueObjects;

public sealed class Period : ValueObject
{
    private Period()
    {
    }
    
    public Period(DateOnly end)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (end < today)
        {
            throw new ArgumentException("End date must be in the future.", nameof(end));
        }

        Start = today.AddDays(1);
        End = end;
    }

    public DateOnly Start { get; }

    public DateOnly End { get; }

    public int Days => End.DayNumber - Start.DayNumber;
    
    public bool IsPast => End < DateOnly.FromDateTime(DateTime.UtcNow);

    public override string ToString() => $"{Start} - {End}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}