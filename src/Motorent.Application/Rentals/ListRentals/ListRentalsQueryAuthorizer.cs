using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;

namespace Motorent.Application.Rentals.ListRentals;

internal sealed class ListRentalsQueryAuthorizer : IAuthorizer<ListRentalsQuery>
{
    public IEnumerable<IRequirement> GetRequirements(ListRentalsQuery _)
    {
        yield return new RoleRequirement(UserRoles.Renter);
    }
}