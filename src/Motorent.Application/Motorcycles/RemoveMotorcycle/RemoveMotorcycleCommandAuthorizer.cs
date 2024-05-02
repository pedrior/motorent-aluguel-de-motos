using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Motorcycles.RemoveMotorcycle;

internal sealed class RemoveMotorcycleCommandAuthorizer : IAuthorizer<RemoveMotorcycleCommand>
{
    public IEnumerable<IRequirement> GetRequirements(RemoveMotorcycleCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Admin);
    }
}