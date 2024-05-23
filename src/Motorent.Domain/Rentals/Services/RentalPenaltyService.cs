using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.Rentals.Services;

public sealed class RentalPenaltyService : IRentalPenaltyService
{
    private const decimal DaysInAdvancePenaltyRate = 0.4m;
    private const decimal PostponementPenaltyPerDay = 50.0m;

    public Money Calculate(DateOnly currentReturnDate, DateOnly newReturnDate)
    {
        if (currentReturnDate == newReturnDate)
        {
            return Money.Zero;
        }

        return IsPostponing(currentReturnDate, newReturnDate)
            ? CalculatePostponementPenalty(currentReturnDate, newReturnDate)
            : CalculateAdvancePenalty(currentReturnDate, newReturnDate);
    }

    private static bool IsPostponing(DateOnly currentReturnDate, DateOnly newReturnDate) =>
        newReturnDate > currentReturnDate;

    private static Money CalculatePostponementPenalty(DateOnly currentReturnDate, DateOnly newReturnDate)
    {
        var postponedDays = newReturnDate.DayNumber - currentReturnDate.DayNumber;
        return new Money(PostponementPenaltyPerDay * postponedDays);
    }

    private static Money CalculateAdvancePenalty(DateOnly currentReturnDate, DateOnly newReturnDate)
    {
        var daysInAdvance = currentReturnDate.DayNumber - newReturnDate.DayNumber;
        return new Money(daysInAdvance * DaysInAdvancePenaltyRate);
    }
}