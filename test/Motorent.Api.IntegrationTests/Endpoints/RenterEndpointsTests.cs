using Motorent.Domain.Renters;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RenterEndpoints))]
public sealed partial class RenterEndpointsTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private async Task<Renter> CreateRenterAsync(string userId, string? driverLicenseNumber = null)
    {
        var renter = (await Factories.Renter.CreateAsync(
            userId: userId,
            driverLicense: DriverLicense.Create(
                number: driverLicenseNumber ?? Constants.Renter.DriverLicense.Number,
                category: Constants.Renter.DriverLicense.Category,
                expiry: Constants.Renter.DriverLicense.Expiry).Value)).Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.SaveChangesAsync();

        return renter;
    }
}