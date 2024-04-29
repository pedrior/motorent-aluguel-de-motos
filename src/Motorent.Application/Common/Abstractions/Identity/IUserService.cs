namespace Motorent.Application.Common.Abstractions.Identity;

public interface IUserService
{
    Task<Result<string>> CreateUserAsync(
        string email,
        string password,
        string[]? roles = null,
        IDictionary<string, string>? claims = null,
        CancellationToken cancellationToken = default);

    Task<Result<string>> CheckPasswordAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
}