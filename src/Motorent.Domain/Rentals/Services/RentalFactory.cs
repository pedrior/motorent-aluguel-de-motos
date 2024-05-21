using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.Errors;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Enums;

namespace Motorent.Domain.Rentals.Services;

public sealed class RentalFactory : IRentalFactory
{
    public Result<Rental> Create(Renter renter, RentalId id, MotorcycleId motorcycleId, RentalPlan plan)
    {
        if (!renter.DriverLicense.CanDrive(DriverLicenseCategory.A))
        {
            return RentalErrors.RenterMustHaveCategoryADrivingLicense;
        }

        return Rental.Create(id, renter.Id, motorcycleId, plan);
    }
}