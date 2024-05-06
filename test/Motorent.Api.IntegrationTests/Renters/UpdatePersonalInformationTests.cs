using Motorent.Presentation.Renters;
using Motorent.TestUtils.Factories;

namespace Motorent.Api.IntegrationTests.Renters;

[TestSubject(typeof(UpdatePersonalInformation))]
public sealed class UpdatePersonalInformationTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task UpdatePersonalInformation_WhenRequested_ShouldUpdatePersonalInformation()
    {
        // Arrange
        var userId = await CreateUserAsync(TestUser.Renter);
        await AuthenticateUserAsync(userId);
        
        await CreateRenterAsync(userId);
        
        var request = Requests.Renter.UpdatePersonalInformation();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DataContext.ChangeTracker.Clear();
        
        var renter = await DataContext.Renters.SingleAsync(r => r.UserId == userId);
        
        renter.FullName.GivenName.Should().Be(Requests.Renter.UpdatePersonalInformationRequest.GivenName);
        renter.FullName.FamilyName.Should().Be(Requests.Renter.UpdatePersonalInformationRequest.FamilyName);
        renter.Birthdate.Value.Should().Be(Requests.Renter.UpdatePersonalInformationRequest.Birthdate);
    }
    
    [Fact]
    public async Task UpdatePersonalInformation_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Renter.GetRenterProfile();
        
        // Act
        var response = await Client.SendAsync(request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UpdatePersonalInformation_WhenRequestedByNonRenter_ShouldReturnForbidden()
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
    
    private async Task CreateRenterAsync(string userId)
    {
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;
        
        DataContext.Renters.Add(renter);
        await DataContext.SaveChangesAsync();
    }
}