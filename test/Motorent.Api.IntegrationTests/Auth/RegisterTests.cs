using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Motorent.Contracts.Auth.Responses;
using Motorent.Infrastructure.Common.Security;
using Motorent.Presentation.Auth;

namespace Motorent.Api.IntegrationTests.Auth;

[TestSubject(typeof(Register))]
public sealed class RegisterTests(WebApplicationFactory api) : WebApplicationFixture(api)
{
    [Fact]
    public async Task Register_WhenCommandIsValid_ShouldReturnCreated()
    {
        // Arrange
        var request = Requests.Auth.Register();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var user = await DataContext.Users.SingleOrDefaultAsync(
            u => u.Email == Requests.Auth.RegisterRequest.Email);

        user.Should().NotBeNull();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
            content, SerializationOptions.Options);

        var expiryInSeconds = GetRequiredService<IConfiguration>()
            .GetValue<int>($"{SecurityTokenOptions.SectionName}:" +
                           $"{nameof(SecurityTokenOptions.ExpiryInSeconds)}");

        tokenResponse.Should().NotBeNull();
        tokenResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokenResponse.ExpiresIn.Should().Be(expiryInSeconds);
    }

    [Fact]
    public async Task Register_WhenEmailIsNotUnique_ShouldReturnConflict()
    {
        // Arrange
        await CreateUserAsync(TestUser.Renter with
        {
            Email = Requests.Auth.RegisterRequest.Email
        });

        var request = Requests.Auth.Register();

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}