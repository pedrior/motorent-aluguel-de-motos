using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RenterEndpoints))]
public sealed partial class RenterEndpointsTests
{
    [Fact]
    public async Task UpdateDriverLicense_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        await AuthenticateUserAsync(userId);

        await CreateRenterAsync(userId);

        var request = Requests.Renter.UpdateDriverLicense();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();

        var renter = await DataContext.Renters.SingleAsync(r => r.UserId == userId);

        renter.DriverLicense.Number.Should().Be(Requests.Renter.UpdateDriverLicenseRequest.Number);
        renter.DriverLicense.Category.Name.Should().BeEquivalentTo(Requests.Renter.UpdateDriverLicenseRequest.Category);
        renter.DriverLicense.Expiry.Should().Be(Requests.Renter.UpdateDriverLicenseRequest.ExpDate);
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenDriverLicenseIsDuplicate_ShouldReturnConflict()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        await AuthenticateUserAsync(userId);

        await CreateRenterAsync(userId, driverLicenseNumber: Requests.Renter.UpdateDriverLicenseRequest.Number);

        var request = Requests.Renter.UpdateDriverLicense();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var count = await DataContext.Renters.CountAsync(
            r => r.DriverLicense.Number == Requests.Renter.UpdateDriverLicenseRequest.Number);

        count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.UpdateDriverLicense();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Admin]);
        await AuthenticateUserAsync(userId);

        var request = Requests.Renter.UpdateDriverLicense();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}