using Microsoft.Extensions.Configuration;
using Motorent.Api.IntegrationTests.TestUtils;
using Motorent.Contracts.Auth.Requests;
using Motorent.Contracts.Auth.Responses;
using Motorent.Infrastructure.Common.Security;

namespace Motorent.Api.IntegrationTests.Endpoints.Auth;

[TestSubject(typeof(AuthEndpoints))]
public sealed class LoginTests(IntegrationTestWebApplicationFactory api) : AbstractIntegrationTest(api)
{
    private static readonly LoginRequest LoginRequest = new()
    {
        Email = "john@doe.com",
        Password = "JohnDoe123"
    };

    public override async Task InitializeAsync()
    {
        await CreateUserAsync(
            email: LoginRequest.Email,
            password: LoginRequest.Password);

        await base.InitializeAsync();
    }

    [Fact]
    public async Task Login_WhenRequestIsValid_ShouldReturnTokenResponse()
    {
        // Arrange
        var request = Requests.Auth.Login(LoginRequest);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.DeserializeContentAsync<TokenResponse>();

        content.Should().NotBeNull();
        content.Type.Should().Be("Bearer");
        content.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.ExpiresIn.Should().Be(GetRequiredService<IConfiguration>()
            .GetValue<int>($"{SecurityTokenOptions.SectionName}:" +
                           $"{nameof(SecurityTokenOptions.ExpiryInSeconds)}"));
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Auth.Login(LoginRequest with { Email = "jane@doe.com" });

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WhenPasswordIsIncorrect_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Auth.Login(LoginRequest with { Password = "JaneDoe123" });

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}