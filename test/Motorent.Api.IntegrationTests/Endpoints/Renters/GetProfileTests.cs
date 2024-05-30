using Motorent.Api.IntegrationTests.TestUtils;
using Motorent.Contracts.Renters.Responses;

namespace Motorent.Api.IntegrationTests.Endpoints.Renters;

[TestSubject(typeof(RenterEndpoints))]
public sealed class GetProfileTests(IntegrationTestWebApplicationFactory api) : AbstractIntegrationTest(api)
{
    public override async Task InitializeAsync()
    {
        var userId = await CreateUserAsync(roles: [RenterUserRole], authenticate: true);
        
        await CreateRenterAsync(userId);

        await base.InitializeAsync();
    }

    private async Task CreateRenterAsync(string userId)
    {
        var renter = (await Factories.Renter.CreateAsync(userId: userId)).Value;

        await DataContext.Renters.AddAsync(renter);
        await DataContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetProfile_WhenRequestIsValid_ShouldReturnRenterProfile()
    {
        // Arrange
        var request = Requests.Renter.GetProfile();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.DeserializeContentAsync<RenterProfileResponse>();

        content.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProfile_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthentication();

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
        await CreateUserAsync(
            email: "jane@admin.com",
            roles: [AdminUserRole],
            authenticate: true);

        var request = Requests.Renter.GetProfile();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}