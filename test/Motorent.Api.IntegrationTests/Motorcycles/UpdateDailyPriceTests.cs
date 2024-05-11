using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Presentation.Motorcycles;

namespace Motorent.Api.IntegrationTests.Motorcycles;

[TestSubject(typeof(UpdateDailyPrice))]
public sealed class UpdateDailyPriceTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task UpdateDailyPrice_WhenRequestIsValid_ShouldChangeDailyPrice()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Admin));
        
        var motorcycleId = await CreateMotorcycleAsync();

        var request = Requests.Motorcycle.UpdateDailyPrice(motorcycleId.ToString());
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        DataContext.ChangeTracker.Clear();
        
        var motorcycle = await DataContext.Motorcycles.FindAsync([motorcycleId]);
        motorcycle.Should().NotBeNull();
        
        motorcycle!.DailyPrice.Value.Should().Be(Requests.Motorcycle.UpdateDailyPriceRequest.DailyPrice);
    }
    
    [Fact]
    public async Task UpdateDailyPrice_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var motorcycleId = await CreateMotorcycleAsync();

        var request = Requests.Motorcycle.UpdateDailyPrice(motorcycleId.ToString());
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UpdateDailyPrice_WhenUserIsNotAdmin_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateUserAsync(await CreateUserAsync(TestUser.Renter));
        
        var motorcycleId = await CreateMotorcycleAsync();

        var request = Requests.Motorcycle.UpdateDailyPrice(motorcycleId.ToString());
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    private async Task<MotorcycleId> CreateMotorcycleAsync()
    {
        var motorcycle = await Factories.Motorcycle.CreateAsync();
        await DataContext.Motorcycles.AddAsync(motorcycle.Value);
        await DataContext.SaveChangesAsync();

        return motorcycle.Value.Id;
    }
}