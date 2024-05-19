using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Renters.Common.Storage;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.UploadDriverLicenseImage;

internal sealed class UploadDriverLicenseImageCommandHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IStorageService storageService) : ICommandHandler<UploadDriverLicenseImageCommand>
{
    public async Task<Result<Success>> Handle(UploadDriverLicenseImageCommand command,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        var imageUrl = await UploadDriverLicenseImageAsync(renter.Id, command.Image, cancellationToken);
        return await renter.SendDriverLicenseImage(imageUrl)
            .ThenAsync(() => renterRepository.UpdateAsync(renter, cancellationToken));
    }

    private async Task<Uri> UploadDriverLicenseImageAsync(
        RenterId renterId,
        IFile image,
        CancellationToken cancellationToken)
    {
        var imagePath = RenterStorageUtils.GetDriverLicenseImagePath(renterId, image.Extension);
        await storageService.UploadAsync(imagePath, image, cancellationToken);

        return imagePath;
    }
}