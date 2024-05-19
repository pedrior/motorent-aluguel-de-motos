using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RenterEndpoints))]
public sealed partial class RenterEndpointsTests
{
    [Fact]
    public async Task UpdatePersonalInfo_WhenRequested_ShouldUpdatePersonalInfo()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        await AuthenticateUserAsync(userId);

        await CreateRenterAsync(userId);

        var request = Requests.Renter.UpdatePersonalInfo();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();

        var renter = await DataContext.Renters.SingleAsync(r => r.UserId == userId);

        renter.FullName.GivenName.Should().Be(Requests.Renter.UpdatePersonalInfoRequest.GivenName);
        renter.FullName.FamilyName.Should().Be(Requests.Renter.UpdatePersonalInfoRequest.FamilyName);
        renter.Birthdate.Value.Should().Be(Requests.Renter.UpdatePersonalInfoRequest.Birthdate);
    }

    [Fact]
    public async Task UpdatePersonalInfo_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.GetProfile();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePersonalInfo_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Admin]);
        await AuthenticateUserAsync(userId);

        var request = Requests.Renter.GetProfile();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}