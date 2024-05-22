using Motorent.Contracts.Renters.Requests;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Renters;

[TestSubject(typeof(RenterEndpoints))]
public sealed class UpdateDriverLicenseTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private string userId = null!;
    private UpdateDriverLicenseRequest updateDriverLicenseRequest = null!;

    public override async Task InitializeAsync()
    {
        updateDriverLicenseRequest = new UpdateDriverLicenseRequest
        {
            Number = "19452106448",
            Category = "B",
            Expiry = new DateOnly(DateTime.Today.Year + 2, 10, 25)
        };

        userId = await CreateUserAsync(roles: [RenterUserRole], authenticate: true);

        await base.InitializeAsync();
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        await CreateRenterAsync();

        var request = Requests.Renter.UpdateDriverLicense(updateDriverLicenseRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();

        var renter = await DataContext.Renters.SingleAsync(r => r.UserId == userId);

        renter.DriverLicense.Number.Should().Be(updateDriverLicenseRequest.Number);
        renter.DriverLicense.Category.Name.Should().BeEquivalentTo(updateDriverLicenseRequest.Category);
        renter.DriverLicense.Expiry.Should().Be(updateDriverLicenseRequest.Expiry);
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenDriverLicenseIsDuplicate_ShouldReturnConflict()
    {
        // Arrange
        await CreateRenterAsync(updateDriverLicenseRequest.Number);

        var request = Requests.Renter.UpdateDriverLicense(updateDriverLicenseRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var count = await DataContext.Renters.CountAsync(
            r => r.DriverLicense.Number == updateDriverLicenseRequest.Number);

        count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Renter.UpdateDriverLicense(updateDriverLicenseRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateDriverLicense_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "jane@admin.com",
            roles: [AdminUserRole],
            authenticate: true);

        var request = Requests.Renter.UpdateDriverLicense(updateDriverLicenseRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task CreateRenterAsync(string? driverLicenseNumber = null)
    {
        var renter = (await Factories.Renter.CreateAsync(
            userId: userId,
            driverLicense: DriverLicense.Create(
                number: driverLicenseNumber ?? Constants.Renter.DriverLicense.Number,
                category: Constants.Renter.DriverLicense.Category,
                expiry: Constants.Renter.DriverLicense.Expiry).Value)).Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.SaveChangesAsync();
    }
}