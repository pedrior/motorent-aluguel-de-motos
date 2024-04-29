using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Motorent.Application.Common.Abstractions.Identity;

namespace Motorent.Infrastructure.Common.Identity;

internal sealed class UserContext : IUserContext
{
    public UserContext(IHttpContextAccessor accessor)
    {
        var principal = accessor.HttpContext?.User;
        if (principal is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        UserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
    }

    public string UserId { get; } = string.Empty;
}