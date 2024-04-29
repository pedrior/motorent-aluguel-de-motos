using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;

namespace Motorent.Application.Common.Security;

internal sealed class RoleRequirementHandler(IUserContext userContext, IUserService userService)
    : IRequirementHandler<RoleRequirement>
{
    public Task<bool> AuthorizeAsync(RoleRequirement requirement, CancellationToken cancellationToken) =>
        userService.IsInRoleAsync(userContext.UserId, requirement.Role, cancellationToken);
}