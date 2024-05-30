using Motorent.Api.IntegrationTests.TestUtils;
using Motorent.Contracts.Renters.Requests;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Renters;

[TestSubject(typeof(RenterEndpoints))]
public sealed class UpdatePersonalInfoTests(IntegrationTestWebApplicationFactory api) : AbstractIntegrationTest(api)
{
    private static readonly UpdatePersonalInfoRequest UpdatePersonalInfoRequest = new()
    {
        GivenName = "Jane",
        FamilyName = "Doe",
        Birthdate = new DateOnly(1990, 1, 1)
    };
    
    [Fact]
    public async Task UpdatePersonalInfo_WhenRequestIsValid_ShouldUpdatePersonalInfo()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [RenterUserRole], authenticate: true);

        await CreateRenterAsync(userId);

        var request = Requests.Renter.UpdatePersonalInfo(UpdatePersonalInfoRequest);

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();

        var renter = await DataContext.Renters.SingleAsync(r => r.UserId == userId);

        renter.FullName.GivenName.Should().Be(UpdatePersonalInfoRequest.GivenName);
        renter.FullName.FamilyName.Should().Be(UpdatePersonalInfoRequest.FamilyName);
        renter.Birthdate.Value.Should().Be(UpdatePersonalInfoRequest.Birthdate);
    }

    [Fact]
    public async Task UpdatePersonalInfo_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.GetProfile();

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePersonalInfo_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(roles: [AdminUserRole], authenticate: true);
        
        var request = Requests.Renter.GetProfile();

        // Act
        var message = await Client.SendAsync(request);

        // Assert
        message.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    private async Task CreateRenterAsync(string userId, string? driverLicenseNumber = null)
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