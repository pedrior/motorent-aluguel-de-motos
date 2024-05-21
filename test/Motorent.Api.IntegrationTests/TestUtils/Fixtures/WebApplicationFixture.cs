using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Api.IntegrationTests.TestUtils.Fixtures;

public abstract class WebApplicationFixture(WebApplicationFactory api)
    : IClassFixture<WebApplicationFactory>, IAsyncLifetime
{
    protected static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    
    private readonly IServiceScope serviceScope = api.Services.CreateScope();

    private HttpClient? client;
    private DataContext? dataContext;

    protected HttpClient Client => client ??= api.CreateClient();

    internal DataContext DataContext => dataContext ??= GetRequiredService<DataContext>();

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        if (client is not null)
        {
            client.DefaultRequestHeaders.Authorization = null;
        }

        DataContext.ChangeTracker.Clear();

        serviceScope.Dispose();

        await api.ResetDatabaseAsync();
    }

    protected T GetRequiredService<T>() where T : notnull => serviceScope.ServiceProvider.GetRequiredService<T>();

    protected async Task<string> CreateUserAsync(
        string? email = null,
        string? password = null,
        string[]? roles = null,
        Dictionary<string, string>? claims = null)
    {
        var user = new User(
            email ?? "john@doe.com",
            PasswordHelper.Hash(password ?? "JohnDoe123"),
            roles ?? [UserRoles.Admin],
            claims ?? new Dictionary<string, string>
            {
                [JwtRegisteredClaimNames.GivenName] = "John",
                [JwtRegisteredClaimNames.FamilyName] = "Doe",
                [JwtRegisteredClaimNames.Birthdate] = "2000-09-05"
            });

        await DataContext.Users.AddAsync(user);
        await DataContext.SaveChangesAsync();

        return user.Id;
    }

    protected async Task AuthenticateUserAsync(string userId)
    {
        var securityTokenProvider = GetRequiredService<ISecurityTokenProvider>();
        var securityToken = await securityTokenProvider.GenerateSecurityTokenAsync(userId);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", securityToken.AccessToken);
    }
    
    protected void ClearAuthentication() => Client.DefaultRequestHeaders.Authorization = null;
}