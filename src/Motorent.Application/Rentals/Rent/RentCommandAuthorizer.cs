using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Rentals.Rent;

internal sealed class RentCommandAuthorizer : IAuthorizer<RentCommand>
{
    public IEnumerable<IRequirement> GetRequirements(RentCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}