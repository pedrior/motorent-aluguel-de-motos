using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.UpdatePersonalInformation;

internal sealed class UpdatePersonalInformationCommandAuthorizer : IAuthorizer<UpdatePersonalInformationCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdatePersonalInformationCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}