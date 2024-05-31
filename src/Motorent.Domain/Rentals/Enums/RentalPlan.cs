using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.Rentals.Enums;

public sealed class RentalPlan : Enum<RentalPlan>
{
    public static readonly RentalPlan SevenDays = new("7-days", 1)
    {
        Days = 7,
        DailyPrice = new Money(30m)
    };

    public static readonly RentalPlan FifteenDays = new("15-days", 2)
    {
        Days = 15,
        DailyPrice = new Money(28m)
    };

    public static readonly RentalPlan ThirtyDays = new("30-days", 3)
    {
        Days = 30,
        DailyPrice = new Money(22m)
    };

    public static readonly RentalPlan FortyFiveDays = new("45-days", 4)
    {
        Days = 45,
        DailyPrice = new Money(20m)
    };

    public static readonly RentalPlan FiftyDays = new("50-days", 5)
    {
        Days = 50,
        DailyPrice = new Money(18m)
    };

    private RentalPlan(string name, int value) : base(name, value)
    {
    }

    public required int Days { get; init; }

    public required Money DailyPrice { get; init; }
    
    public Money CalculateTotalPrice() => DailyPrice * Days;
}