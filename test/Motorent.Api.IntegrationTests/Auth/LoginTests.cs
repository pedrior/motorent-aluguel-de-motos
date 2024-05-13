using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Motorent.Contracts.Auth.Responses;
using Motorent.Infrastructure.Common.Security;
using Motorent.Presentation.Auth;

namespace Motorent.Api.IntegrationTests.Auth;

[TestSubject(typeof(Login))]
public sealed class LoginTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task Login_WhenCommandIsValid_ShouldReturnOk()
    {
        // Arrange
        await CreateUserAsync(
            email: Requests.Auth.LoginRequest.Email,
            password: Requests.Auth.LoginRequest.Password);
        
        var request = Requests.Auth.Login();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, SerializerOptions);

        var expiryInSeconds = GetRequiredService<IConfiguration>()
            .GetValue<int>($"{SecurityTokenOptions.SectionName}:" +
                           $"{nameof(SecurityTokenOptions.ExpiryInSeconds)}");

        tokenResponse.Should().NotBeNull();
        tokenResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokenResponse.ExpiresIn.Should().Be(expiryInSeconds);
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = Requests.Auth.Login();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WhenPasswordIsIncorrect_ShouldReturnUnauthorized()
    {
        // Arrange
        await CreateUserAsync(
            email: Requests.Auth.LoginRequest.Email,
            password: "password");

        var request = Requests.Auth.Login();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}