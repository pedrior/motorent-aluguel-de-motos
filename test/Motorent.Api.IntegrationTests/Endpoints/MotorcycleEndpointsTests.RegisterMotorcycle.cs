using Motorent.Contracts.Motorcycles.Responses;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed partial class MotorcycleEndpointsTests
{
    [Fact]
    public async Task RegisterMotorcycle_WhenRequestIsValid_ShouldCreateMotorcycle()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Admin]));

        var request = Requests.Motorcycle.RegisterMotorcycle();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var motorcycleResponse = await response.Content.ReadFromJsonAsync<MotorcycleResponse>();
        var motorcycle = await DataContext.Motorcycles.SingleAsync(
            m => m.Id == new MotorcycleId(motorcycleResponse!.Id));

        motorcycle.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterMotorcycle_WhenLicensePlateIsDuplicated_ShouldReturnConflict()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(roles: [UserRoles.Admin]));

        // Register a motorcycle with the default license plate
        await Client.SendAsync(Requests.Motorcycle.RegisterMotorcycle());

        var request = Requests.Motorcycle.RegisterMotorcycle();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}