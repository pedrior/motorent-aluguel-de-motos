using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.Events;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.Errors;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Rentals;

public sealed class Rental : Entity<RentalId>, IAggregateRoot, IAuditable
{
    private Rental(RentalId id) : base(id)
    {
    }

    public required RenterId RenterId { get; init; }

    public required MotorcycleId MotorcycleId { get; init; }

    public required RentalPlan Plan { get; init; }

    public required Period Period { get; init; }

    public DateOnly ReturnDate { get; private set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    
    public bool IsActive => !Period.IsPast;
    
    public Money Penalty { get; private set; } = null!;

    public Money DailyPrice => Plan.DailyPrice;
    
    public Money TotalPrice => Plan.CalculateTotalPrice() + Penalty;

    internal static Rental Create(
        RentalId id,
        RenterId renterId,
        MotorcycleId motorcycleId,
        RentalPlan plan)
    {
        var period = GetPeriodForPlan(plan);
        var rental = new Rental(id)
        {
            RenterId = renterId,
            MotorcycleId = motorcycleId,
            Plan = plan,
            Period = period,
            ReturnDate = period.End,
            Penalty = Money.Zero
        };
        
        rental.RaiseEvent(new RentalCreated(id));
        
        return rental;
    }

    public Result<Success> ChangeReturnDate(DateOnly newReturnDate,
        IRentalPenaltyService rentalPenaltyService)
    {
        if (!IsActive)
        {
            return RenterErrors.RentalIsNotActive;
        }

        if (newReturnDate < Period.Start)
        {
            return RenterErrors.ReturnDateIsBeforeRentalStartDate;
        }

        Penalty = rentalPenaltyService.Calculate(ReturnDate, newReturnDate);
        ReturnDate = newReturnDate;

        return Success.Value;
    }

    private static Period GetPeriodForPlan(RentalPlan plan) =>
        new(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(plan.Days + 1));
}