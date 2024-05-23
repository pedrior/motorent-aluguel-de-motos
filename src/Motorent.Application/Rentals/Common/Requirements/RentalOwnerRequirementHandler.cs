using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Rentals.Common.Requirements;

internal sealed class RentalOwnerRequirementHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IRentalRepository rentalRepository) : IRequirementHandler<RentalOwnerRequirement>
{
    public async Task<bool> AuthorizeAsync(RentalOwnerRequirement requirement, CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        var rental = await rentalRepository.FindAsync(requirement.RentalId, cancellationToken);

        return renter is not null && (rental is null || rental.RenterId == renter.Id);
    }
}