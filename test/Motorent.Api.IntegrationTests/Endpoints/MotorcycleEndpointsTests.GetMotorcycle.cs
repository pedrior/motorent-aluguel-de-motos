using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed partial class MotorcycleEndpointsTests
{
    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleExistsById_ShouldReturnOk()
    {
        // Arrange
        var motorcycleId = await CreateMotorcycleAsync();
        var request = Requests.Motorcycle.GetMotorcycle(motorcycleId.ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleExistsByLicensePlate_ShouldReturnOk()
    {
        // Arrange
        var licensePlate = Constants.Motorcycle.LicensePlate;
        await CreateMotorcycleAsync(licensePlate: licensePlate);

        var request = Requests.Motorcycle.GetMotorcycle(licensePlate.ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleDoesNotExistById_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Motorcycle.GetMotorcycle(Ulid.NewUlid().ToString());

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleDoesNotExistByLicensePlate_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Motorcycle.GetMotorcycle("JPH8H87");

        // Act
        var result = await Client.SendAsync(request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}