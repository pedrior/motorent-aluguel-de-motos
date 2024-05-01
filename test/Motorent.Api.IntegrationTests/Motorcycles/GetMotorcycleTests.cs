using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Presentation.Motorcycles;
using Motorent.TestUtils.Constants;
using Motorent.TestUtils.Factories;

namespace Motorent.Api.IntegrationTests.Motorcycles;

[TestSubject(typeof(GetMotorcycle))]
public sealed class GetMotorcycleTests(WebApplicationFactory api) : WebApplicationFixture(api)
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
        var request = Requests.Motorcycle.GetMotorcycle("JPH-8H87");
        
        // Act
        var result = await Client.SendAsync(request);
        
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<MotorcycleId> CreateMotorcycleAsync(LicensePlate? licensePlate = null)
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync(licensePlate: licensePlate);
        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();

        return motorcycle.Value.Id;
    }
}