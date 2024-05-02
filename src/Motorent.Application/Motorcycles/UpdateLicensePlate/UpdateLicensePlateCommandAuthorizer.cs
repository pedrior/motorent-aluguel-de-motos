using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Motorcycles.UpdateLicensePlate;

internal sealed class UpdateLicensePlateCommandAuthorizer : IAuthorizer<UpdateLicensePlateCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdateLicensePlateCommand _)
    {
        yield return new RoleRequirement(UserRoles.Admin);
    }
}