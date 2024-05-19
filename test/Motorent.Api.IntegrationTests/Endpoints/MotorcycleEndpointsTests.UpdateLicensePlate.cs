using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed partial class MotorcycleEndpointsTests
{
    [Fact]
    public async Task UpdateLicensePlate_WhenMotorcycleExists_ShouldReturnNoContent()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Admin]));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.UpdateLicensePlate(motorcycleId.ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateLicensePlate_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.UpdateLicensePlate(motorcycleId.ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangeLicensePlace_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Renter]));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.UpdateLicensePlate(motorcycleId.ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateLicensePlate_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Admin]));

        var request = Requests.Motorcycle.UpdateLicensePlate(Ulid.NewUlid().ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}