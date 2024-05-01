using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Motorcycles.Register;

internal sealed class RegisterMotorcycleCommandAuthorizer : IAuthorizer<RegisterMotorcycleCommand>
{
    public IEnumerable<IRequirement> GetRequirements(RegisterMotorcycleCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Admin);
    }
}