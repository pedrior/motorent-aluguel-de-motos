using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;
using Motorent.Application.Rentals.Common.Requirements;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Rentals.GetRental;

internal sealed class GetRentalQueryAuthorizer : IAuthorizer<GetRentalQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetRentalQuery query)
    {
        yield return new RoleRequirement(UserRoles.Renter);
        yield return new RentalOwnerRequirement(new RentalId(query.Id));
    }
}