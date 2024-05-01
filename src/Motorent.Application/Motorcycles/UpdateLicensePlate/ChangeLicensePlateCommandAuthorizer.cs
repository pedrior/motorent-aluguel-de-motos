using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Motorcycles.UpdateLicensePlate;

internal sealed class ChangeLicensePlateCommandAuthorizer : IAuthorizer<ChangeLicensePlateCommand>
{
    public IEnumerable<IRequirement> GetRequirements(ChangeLicensePlateCommand _)
    {
        yield return new RoleRequirement(UserRoles.Admin);
    }
}