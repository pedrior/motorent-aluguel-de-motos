using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Api.IntegrationTests.TestUtils;

public abstract class AbstractIntegrationTest(IntegrationTestWebApplicationFactory api)
    : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    protected const string AdminUserRole = "admin";
    protected const string RenterUserRole = "renter";

    protected static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly IServiceScope serviceScope = api.Services.CreateScope();

    private HttpClient? client;

    protected HttpClient Client => client ??= api.CreateClient();

    internal DataContext DataContext => GetRequiredService<DataContext>();

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        await api.ResetDatabaseAsync();

        serviceScope.Dispose();

        ClearAuthentication();
    }

    protected T GetRequiredService<T>() where T : notnull =>
        serviceScope.ServiceProvider.GetRequiredService<T>();

    protected async Task<string> CreateUserAsync(
        string? email = null,
        string? password = null,
        string[]? roles = null,
        Dictionary<string, string>? claims = null,
        bool authenticate = false)
    {
        var user = new User(
            email ?? "john@doe.com",
            PasswordHelper.Hash(password ?? "JohnDoe123"),
            roles ?? [AdminUserRole],
            claims ?? new Dictionary<string, string>
            {
                [JwtRegisteredClaimNames.GivenName] = "John",
                [JwtRegisteredClaimNames.FamilyName] = "Doe",
                [JwtRegisteredClaimNames.Birthdate] = "2000-09-05"
            });

        await DataContext.Users.AddAsync(user);
        await DataContext.SaveChangesAsync();

        if (authenticate)
        {
            await AuthenticateAsync(user.Id);
        }

        return user.Id;
    }

    protected void ClearAuthentication()
    {
        if (client is not null)
        {
            client.DefaultRequestHeaders.Authorization = null;
        }
    }

    private async Task AuthenticateAsync(string userId)
    {
        var securityTokenProvider = GetRequiredService<ISecurityTokenProvider>();
        var securityToken = await securityTokenProvider.GenerateSecurityTokenAsync(userId);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", securityToken.AccessToken);
    }
}