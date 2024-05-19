using Mapster;
using Motorent.Contracts.Renters.Responses;
using Motorent.Presentation.Endpoints;

namespace Motorent.Api.IntegrationTests.Endpoints;

[TestSubject(typeof(RenterEndpoints))]
public sealed partial class RenterEndpointsTests
{
    [Fact]
    public async Task GetProfile_WhenRequested_ShouldReturnRenterProfile()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Renter]);
        var renter = await CreateRenterAsync(userId);
        
        await AuthenticateUserAsync(userId);
        
        var request = Requests.Renter.GetProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var profileResponse = await response.Content.ReadFromJsonAsync<RenterProfileResponse>(SerializerOptions);
        
        profileResponse.Should().NotBeNull();
        profileResponse.Should().Be(renter.Adapt<RenterProfileResponse>());
    }
    
    [Fact]
    public async Task GetProfile_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.GetProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetProfile_WhenRequestedByNonRenter_ShouldReturnForbidden()
    {
        // Arrange
        var userId = await CreateUserAsync(roles: [UserRoles.Admin]);
        await AuthenticateUserAsync(userId);
        
        var request = Requests.Renter.GetProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}