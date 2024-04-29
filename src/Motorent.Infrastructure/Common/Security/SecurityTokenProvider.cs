using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Infrastructure.Common.Identity;
using Motorent.Infrastructure.Common.Persistence;
using SecurityToken = Motorent.Application.Common.Abstractions.Security.SecurityToken;

namespace Motorent.Infrastructure.Common.Security;

internal sealed class SecurityTokenProvider(
    DataContext dataContext,
    TimeProvider timeProvider,
    IOptions<SecurityTokenOptions> options)
    : ISecurityTokenProvider
{
    private const string Algorithm = SecurityAlgorithms.HmacSha256;

    private readonly SecurityTokenOptions options = options.Value;

    public async Task<SecurityToken> GenerateSecurityTokenAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dataContext.Users.FindAsync([userId], cancellationToken);
        if (user is null)
        {
            throw new SecurityTokenException("The user for which the token is being generated does not exist.");
        }

        var claims = CreateUserClaims(user);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)), Algorithm);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddSeconds(options.ExpiryInSeconds);

        var descriptor = new SecurityTokenDescriptor
        {
            Expires = expires,
            NotBefore = now,
            Claims = claims,
            Issuer = options.Issuer,
            Audience = options.Audience,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler
        {
            MapInboundClaims = false,
            SetDefaultTimesOnTokenCreation = false
        };

        var accessToken = handler.CreateToken(descriptor);
        return new SecurityToken(accessToken, options.ExpiryInSeconds);
    }

    private static Dictionary<string, object> CreateUserClaims(User user)
    {
        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            { JwtRegisteredClaimNames.Sub, user.Id }
        };

        if (user.Claims is not null)
        {
            foreach (var (key, value) in user.Claims)
            {
                claims[key] = value;
            }
        }

        claims.Add(UserClaimTypes.Roles, user.Roles ?? []);

        return claims;
    }
}