using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Presentation.Motorcycles;

namespace Motorent.Api.IntegrationTests.Motorcycles;

[TestSubject(typeof(RemoveMotorcycle))]
public class RemoveMotorcycleTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task RemoveMotorcycle_WhenRequestIsValid_ShouldRemoveMotorcycle()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Admin));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.RemoveMotorcycle(motorcycleId.Value);

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();
        
        var motorcycle = await DataContext.Motorcycles.FindAsync([motorcycleId]);
        motorcycle.Should().BeNull();
    }
    
    [Fact]
    public async Task RemoveMotorcycle_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Admin));
        var request = Requests.Motorcycle.RemoveMotorcycle(Ulid.NewUlid());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task RemoveMotorcycle_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.RemoveMotorcycle(motorcycleId.Value);

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveMotorcycle_WhenUserIsNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Renter));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.RemoveMotorcycle(motorcycleId.Value);

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    private async Task<MotorcycleId> CreateMotorcycleAsync()
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync();
        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();

        return motorcycle.Value.Id;
    }
}