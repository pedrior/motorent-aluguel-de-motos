using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Presentation.Motorcycles;
using Motorent.TestUtils.Factories;

namespace Motorent.Api.IntegrationTests.Motorcycles;

[TestSubject(typeof(UpdateLicensePlate))]
public sealed class UpdateLicensePlateTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task ChangeLicensePlate_WhenMotorcycleExists_ShouldReturnNoContent()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Admin));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.UpdateLicensePlate(motorcycleId.ToString());
        
        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeLicensePlate_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
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
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Renter));

        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.UpdateLicensePlate(motorcycleId.ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeLicensePlate_WhenMotorcycleDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Admin));

        var request = Requests.Motorcycle.UpdateLicensePlate(Ulid.NewUlid().ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<MotorcycleId> CreateMotorcycleAsync()
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync();
        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();

        return motorcycle.Value.Id;
    }
}