using Motorent.Api.IntegrationTests.TestUtils;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Api.IntegrationTests.Endpoints.Motorcycles;

[TestSubject(typeof(MotorcycleEndpoints))]
public sealed class GetMotorcycleTests(IntegrationTestWebApplicationFactory api) : AbstractIntegrationTest(api)
{
    private static readonly MotorcycleId MotorcycleId = MotorcycleId.New();
    private static readonly LicensePlate LicensePlate = LicensePlate.Create("KIL8H95").Value;

    public override async Task InitializeAsync()
    {
        await CreateMotorcycleAsync();

        await base.InitializeAsync();
    }

    private async Task CreateMotorcycleAsync()
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync(
            id: MotorcycleId,
            licensePlate: LicensePlate);

        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleExistsById_ShouldReturnOk()
    {
        // Arrange
        var request = Requests.Motorcycle.GetMotorcycle(MotorcycleId.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleExistsByLicensePlate_ShouldReturnOk()
    {
        // Arrange
        var request = Requests.Motorcycle.GetMotorcycle(LicensePlate.ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleDoesNotExistById_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Motorcycle.GetMotorcycle(Ulid.NewUlid().ToString());

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMotorcycle_WhenMotorcycleDoesNotExistByLicensePlate_ShouldReturnNotFound()
    {
        // Arrange
        var request = Requests.Motorcycle.GetMotorcycle("JPH8H87");

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}