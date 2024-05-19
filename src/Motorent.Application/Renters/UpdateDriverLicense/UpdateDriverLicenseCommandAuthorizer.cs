using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UpdateDriverLicense;

internal sealed class UpdateDriverLicenseCommandAuthorizer : IAuthorizer<UpdateDriverLicenseCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdateDriverLicenseCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}