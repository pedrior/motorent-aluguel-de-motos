using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.TestUtils.Constants;

public static partial class Constants
{
    public static class Rental
    {
        public static readonly RentalId Id = RentalId.New();
        
        public static readonly RentalPlan Plan = RentalPlan.ThirtyDays;
    }
}