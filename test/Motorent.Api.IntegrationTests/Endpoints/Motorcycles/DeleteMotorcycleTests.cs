using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Motorcycles;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed class DeleteMotorcycleTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly MotorcycleId MotorcycleId = MotorcycleId.New();

    public override async Task InitializeAsync()
    {
        await CreateMotorcycleAsync();

        await CreateUserAsync(roles: [AdminUserRole], authenticate: true);

        await base.InitializeAsync();
    }

    private async Task CreateMotorcycleAsync()
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync(id: MotorcycleId);

        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();
    }
    
    [Fact]
    public async Task DeleteMotorcycle_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Motorcycle.DeleteMotorcycle(Ulid.NewUlid());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteMotorcycle_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        var request = Requests.Motorcycle.DeleteMotorcycle(MotorcycleId.Value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMotorcycle_WhenUserIsNotAdmin_ShouldReturnForbidden()
    {
        // Arrange
        await CreateUserAsync(
            email: "john@renter.com",
            roles: [RenterUserRole],
            authenticate: true);

        var request = Requests.Motorcycle.DeleteMotorcycle(MotorcycleId.Value);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}