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
        return ValidateDriverLicense(renter)
            .Then(_ => Rental.Create(id, renter.Id, motorcycleId, plan));
    }

    private static Result<Success> ValidateDriverLicense(Renter renter)
    {
        if (renter.DriverLicenseStatus != DriverLicenseStatus.Approved)
        {
            return RentalErrors.RenterMustHaveAnApprovedDriverLicense;
        }

        if (!renter.DriverLicense.CanDrive(DriverLicenseCategory.A))
        {
            return RentalErrors.RenterMustHaveCategoryADrivingLicense;
        }

        return Success.Value;
    }
}