using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RenterEndpoints))]
public sealed partial class RenterEndpointsTests
{
    [Fact]
    public async Task UpdateCNH_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        await AuthenticateUserAsync(userId);

        await CreateRenterAsync(userId);

        var request = Requests.Renter.UpdateCNH();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();

        var renter = await DataContext.Renters.SingleAsync(r => r.UserId == userId);

        renter.CNH.Number.Should().Be(Requests.Renter.UpdateCNHRequest.Number);
        renter.CNH.Category.Name.Should().BeEquivalentTo(Requests.Renter.UpdateCNHRequest.Category);
        renter.CNH.ExpirationDate.Should().Be(Requests.Renter.UpdateCNHRequest.ExpDate);
    }

    [Fact]
    public async Task UpdateCNH_WhenCNHIsDuplicate_ShouldReturnConflict()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        await AuthenticateUserAsync(userId);

        await CreateRenterAsync(userId, cnhNumber: Requests.Renter.UpdateCNHRequest.Number);

        var request = Requests.Renter.UpdateCNH();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var count = await DataContext.Renters.CountAsync(
            r => r.CNH.Number == Requests.Renter.UpdateCNHRequest.Number);

        count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateCNH_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.UpdateCNH();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCNH_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Admin]);
        await AuthenticateUserAsync(userId);

        var request = Requests.Renter.UpdateCNH();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}