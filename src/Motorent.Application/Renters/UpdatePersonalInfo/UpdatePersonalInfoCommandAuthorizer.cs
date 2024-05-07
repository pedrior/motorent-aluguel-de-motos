using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UpdatePersonalInfo;

internal sealed class UpdatePersonalInfoCommandAuthorizer : IAuthorizer<UpdatePersonalInfoCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdatePersonalInfoCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}