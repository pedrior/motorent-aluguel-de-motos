using Mapster;
using Motorent.Contracts.Renters.Responses;
using Motorent.Domain.Renters;
using Motorent.Presentation.Renters;

namespace Motorent.Api.IntegrationTests.Renters;

[TestSubject(typeof(GetRenterProfile))]
public sealed class GetRenterProfileTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task GetRenterProfile_WhenRequested_ShouldReturnRenterProfile()
    {
        // Arrange
        var userId = await CreateUserAsync(TestUser.Renter);
        await AuthenticateUserAsync(userId);
        
        var renter = await CreateRenterAsync(userId);
        
        var request = Requests.Renter.GetRenterProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var profileResponse = await response.Content.ReadFromJsonAsync<RenterProfileResponse>(
            SerializationOptions.Options);
        
        profileResponse.Should().NotBeNull();
        profileResponse.Should().Be(renter.Adapt<RenterProfileResponse>());
    }
    
    [Fact]
    public async Task GetRenterProfile_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.GetRenterProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetRenterProfile_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await CreateUserAsync(TestUser.Admin);
        await AuthenticateUserAsync(userId);
        
        var request = Requests.Renter.GetRenterProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    private async Task<Renter> CreateRenterAsync(string userId)
    {
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;
        
        DataContext.Renters.Add(renter);
        await DataContext.SaveChangesAsync();
        
        return renter;
    }
}