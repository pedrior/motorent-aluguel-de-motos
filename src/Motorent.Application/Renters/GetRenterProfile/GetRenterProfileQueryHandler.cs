using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Contracts.Renters.Responses;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Renters.GetRenterProfile;

internal sealed class GetRenterProfileQueryHandler(
    IUserContext userContext,
    IRenterRepository renterRepository) : IQueryHandler<GetRenterProfileQuery, RenterProfileResponse>
{
    public async Task<Result<RenterProfileResponse>> Handle(GetRenterProfileQuery _,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        return renter?.Adapt<RenterProfileResponse>() ??
               throw new ApplicationException($"Renter not found for user {userContext.UserId}");
    }
}