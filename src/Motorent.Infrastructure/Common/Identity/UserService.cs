using Microsoft.EntityFrameworkCore;
using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Common.Identity;

internal sealed class UserService(DataContext dataContext) : IUserService
{
    public async Task<Result<string>> CreateUserAsync(
        string email,
        string password,
        string[]? roles = null,
        IDictionary<string, string>? claims = null,
        CancellationToken cancellationToken = default)
    {
        if (await IsEmailNotUniqueAsync(email, cancellationToken))
        {
            return UserErrors.DuplicateEmail;
        }

        var passwordHash = PasswordHelper.Hash(password);
        var user = new User(email, passwordHash, roles, claims);

        await dataContext.Users.AddAsync(user, cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<Result<string>> CheckPasswordAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await FindByEmailAsync(email, cancellationToken);
        if (user is null || !PasswordHelper.Verify(password, user.PasswordHash))
        {
            return UserErrors.InvalidCredentials;
        }

        return user.Id;
    }

    public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        return dataContext.Users.AnyAsync(u => u.Id == userId
                                               && u.Roles != null
                                               && EF.Functions.JsonExists(u.Roles, role), cancellationToken);
    }

    private Task<bool> IsEmailNotUniqueAsync(string email, CancellationToken cancellationToken = default) =>
        dataContext.Users.AnyAsync(u => EF.Functions.ILike(u.Email, email), cancellationToken);

    private Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        dataContext.Users.SingleOrDefaultAsync(u => EF.Functions.ILike(u.Email, email), cancellationToken);
}