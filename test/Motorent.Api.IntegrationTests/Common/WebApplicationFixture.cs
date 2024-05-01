using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Api.IntegrationTests.Common;

public abstract class WebApplicationFixture(WebApplicationFactory api)
    : IClassFixture<WebApplicationFactory>, IAsyncLifetime
{
    private readonly IServiceScope serviceScope = api.Services.CreateScope();
    private HttpClient? client;

    protected HttpClient Client => client ??= api.CreateClient();

    internal DataContext DataContext => api.DataContext;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (client is not null)
        {
            client.DefaultRequestHeaders.Authorization = null;
        }

        serviceScope.Dispose();

        await api.ResetDatabaseAsync();
    }

    protected T GetRequiredService<T>() where T : notnull => serviceScope.ServiceProvider.GetRequiredService<T>();

    protected async Task<string> CreateUserAsync(TestUser testUser)
    {
        var user = new User(
            testUser.Email,
            PasswordHelper.Hash(testUser.Password),
            testUser.Roles,
            testUser.Claims);

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
}