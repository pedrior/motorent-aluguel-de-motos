using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Contracts.Renters.Responses;
using Motorent.Domain.Renters.Repository;

namespace Motorent.Application.Renters.GetRenterProfile;

internal sealed class GetRenterProfileQueryHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IStorageService storageService) : IQueryHandler<GetRenterProfileQuery, RenterProfileResponse>
{
    public async Task<Result<RenterProfileResponse>> Handle(GetRenterProfileQuery _,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        var response = renter.Adapt<RenterProfileResponse>();
        var imageUrl = renter.DriverLicenseImageUrl is not null
            ? await storageService.GenerateUrlAsync(renter.DriverLicenseImageUrl)
            : null;

        return response with
        {
            DriverLicense = response.DriverLicense with
            {
                ImageUrl = imageUrl?.ToString()
            }
        };
    }
}