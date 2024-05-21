using Motorent.Domain.Common.Entities;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;
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

    public Money DailyPrice => Plan.DailyPrice;
    
    public Money TotalPrice => Plan.CalculateTotalPrice();

    internal static Rental Create(
        RentalId id,
        RenterId renterId,
        MotorcycleId motorcycleId,
        RentalPlan plan)
    {
        var period = GetPeriodForPlan(plan);
        return new Rental(id)
        {
            RenterId = renterId,
            MotorcycleId = motorcycleId,
            Plan = plan,
            Period = period,
            ReturnDate = period.End
        };
    }
    
    private static Period GetPeriodForPlan(RentalPlan plan) => 
        new(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(plan.Days + 1));
}