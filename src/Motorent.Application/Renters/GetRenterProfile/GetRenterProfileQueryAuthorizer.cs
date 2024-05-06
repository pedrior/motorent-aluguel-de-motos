using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Renters.GetRenterProfile;

internal sealed class GetRenterProfileQueryAuthorizer : IAuthorizer<GetRenterProfileQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetRenterProfileQuery subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}