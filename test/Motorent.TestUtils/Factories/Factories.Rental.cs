using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.TestUtils.Factories;

public static partial class Factories
{
    public static class Rental
    {
        public static Domain.Rentals.Rental Create(
            RentalId? rentalId = null,
            RenterId? renterId = null,
            MotorcycleId? motorcycleId = null,
            RentalPlan? plan = null)
        {
            return Domain.Rentals.Rental.Create(
                rentalId ?? Constants.Constants.Rental.Id,
                renterId ?? Constants.Constants.Renter.Id,
                motorcycleId ?? Constants.Constants.Motorcycle.Id,
                plan ?? Constants.Constants.Rental.Plan);
        }
    }
}