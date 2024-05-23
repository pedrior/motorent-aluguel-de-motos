using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Application.Common.Security;
using Motorent.Application.Rentals.Common.Requirements;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Rentals.UpdateReturnDate;

internal sealed class UpdateReturnDateCommandAuthorizer : IAuthorizer<UpdateReturnDateCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UpdateReturnDateCommand command)
    {
        yield return new RoleRequirement(UserRoles.Renter);
        yield return new RentalOwnerRequirement(new RentalId(command.RentalId));
    }
}