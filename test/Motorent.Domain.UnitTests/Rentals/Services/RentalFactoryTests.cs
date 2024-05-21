using Motorent.Domain.Rentals.Errors;
using Motorent.Domain.Rentals.Services;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.UnitTests.Rentals.Services;

[TestSubject(typeof(RentalFactory))]
public sealed class RentalFactoryTests
{
    [Fact]
    public async Task Create_WhenValuesAreValid_ShouldReturnRental()
    {
        // Arrange
        var factory = new RentalFactory();
        var renter = await GetRenterAsync(DriverLicenseCategory.A);

        // Act
        var result = factory.Create(
            renter,
            Constants.Rental.Id,
            Constants.Motorcycle.Id,
            Constants.Rental.Plan);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeEquivalentTo(new
        {
            Constants.Rental.Id,
            RenterId = renter.Id,
            MotorcycleId = Constants.Motorcycle.Id,
            Constants.Rental.Plan
        });
    }

    [Fact]
    public async Task
        Create_WhenRenterDoesNotHaveCategoryADrivingLicense_ShouldReturnRenterMustHaveCategoryADrivingLicense()
    {
        // Arrange
        var factory = new RentalFactory();
        var renter = await GetRenterAsync(DriverLicenseCategory.B);

        // Act
        var result = factory.Create(
            renter,
            Constants.Rental.Id,
            Constants.Motorcycle.Id,
            Constants.Rental.Plan);

        // Assert
        result.Should().BeFailure(RentalErrors.RenterMustHaveCategoryADrivingLicense);
    }

    private static async Task<Renter> GetRenterAsync(DriverLicenseCategory driverLicenseCategory)
    {
        return (await Factories.Renter.CreateAsync(
                driverLicense: DriverLicense.Create(
                    number: Constants.Renter.DriverLicense.Number,
                    expiry: Constants.Renter.DriverLicense.Expiry,
                    category: driverLicenseCategory).Value))
            .Value;
    }
}