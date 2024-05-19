using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed partial class MotorcycleEndpointsTests
{
    [Fact]
    public async Task DeleteMotorcycle_WhenRequestIsValid_ShouldDeleteMotorcycle()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Admin]));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.DeleteMotorcycle(motorcycleId.Value);

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();
        
        var motorcycle = await DataContext.Motorcycles.FindAsync([motorcycleId]);
        motorcycle.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteMotorcycle_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Admin]));
        var request = Requests.Motorcycle.DeleteMotorcycle(Ulid.NewUlid());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteMotorcycle_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.DeleteMotorcycle(motorcycleId.Value);

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMotorcycle_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Renter]));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.DeleteMotorcycle(motorcycleId.Value);

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}