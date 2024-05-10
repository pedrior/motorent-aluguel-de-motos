using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UpdateCNH;

internal sealed class UpdateCNHCommandAuthorizer : IAuthorizer<UpdateCNHCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdateCNHCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}