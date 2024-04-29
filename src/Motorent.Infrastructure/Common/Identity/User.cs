namespace Motorent.Infrastructure.Common.Identity;

internal sealed class User(
    string email,
    string passwordHash,
    string[]? roles = null,
    IDictionary<string, string>? claims = null)
{
    public string Id { get; init; } = Ulid.NewUlid().ToString();

    public string Email { get; private set; } = email;

    public string PasswordHash { get; private set; } = passwordHash;
    
    public string[]? Roles { get; private set; } = roles;
    
    public IDictionary<string, string>? Claims { get; private set; } = claims;
    
    public bool IsInRole(string role) => Roles?.Contains(role) ?? false;
}