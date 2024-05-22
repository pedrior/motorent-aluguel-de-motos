using Microsoft.Extensions.Configuration;
using Motorent.Contracts.Auth.Requests;
using Motorent.Contracts.Auth.Responses;
using Motorent.Infrastructure.Common.Security;

namespace Motorent.Api.IntegrationTests.Endpoints.Auth;

[TestSubject(typeof(AuthEndpoints))]
public sealed class RegisterTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    private static readonly RegisterRequest RegisterRequest = new()
    {
        Email = "john@doe.con",
        Password = "JohnDoe123",
        GivenName = "John",
        FamilyName = "Doe",
        Birthdate = new DateOnly(DateTime.Today.Year - 18, 01, 01),
        Document = "82825163000178",
        DriverLicenseNumber = "20839215266",
        DriverLicenseCategory = "A",
        DriverLicenseExpiry = new DateOnly(DateTime.Today.Year + 1, 01, 01)
    };

    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldReturnTokenResponse()
    {
        // Arrange
        var request = Requests.Auth.Register(RegisterRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.DeserializeContentAsync<TokenResponse>();

        content.Should().NotBeNull();
        content.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.ExpiresIn.Should().Be(GetRequiredService<IConfiguration>()
            .GetValue<int>($"{SecurityTokenOptions.SectionName}:" +
                           $"{nameof(SecurityTokenOptions.ExpiryInSeconds)}"));
    }

    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldCreateIdentityUser()
    {
        // Arrange
        var request = Requests.Auth.Register(RegisterRequest);

        // Act
        await Client.SendAsync(request);

        // Assert
        var user = await DataContext.Users.SingleOrDefaultAsync(u => u.Email == RegisterRequest.Email);

        user.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldCreateDomainUser()
    {
        // Arrange
        var request = Requests.Auth.Register(RegisterRequest);

        // Act
        await Client.SendAsync(request);

        // Assert
        var user = await DataContext.Users.SingleAsync(u => u.Email == RegisterRequest.Email);
        var renter = await DataContext.Renters.SingleOrDefaultAsync(r => r.UserId == user.Id);

        renter.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WhenEmailIsNotUnique_ShouldReturnConflict()
    {
        // Arrange
        await CreateUserAsync(email: RegisterRequest.Email);

        var request = Requests.Auth.Register(RegisterRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}