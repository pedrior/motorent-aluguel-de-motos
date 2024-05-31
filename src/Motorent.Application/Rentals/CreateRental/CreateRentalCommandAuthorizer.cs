using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Rentals.CreateRental;

internal sealed class CreateRentalCommandAuthorizer : IAuthorizer<CreateRentalCommand>
{
    public IEnumerable<IRequirement> GetRequirements(CreateRentalCommand subject)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}