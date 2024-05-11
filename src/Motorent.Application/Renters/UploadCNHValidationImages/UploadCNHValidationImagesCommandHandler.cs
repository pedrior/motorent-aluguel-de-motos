using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Renters.Common.Storage;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.UploadCNHValidationImages;

internal sealed class UploadCNHValidationImagesCommandHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IStorageService storageService) : ICommandHandler<UploadCNHValidationImagesCommand>
{
    public async Task<Result<Success>> Handle(UploadCNHValidationImagesCommand command,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        var cnhImageUrls = await UploadCNHValidationImagesAsync(
            renter.Id,
            command.FrontImage,
            command.BackImage,
            cancellationToken);

        var cnhValidationImages = new CNHValidationImages(cnhImageUrls.front, cnhImageUrls.back);

        return await renter.SendCNHImagesForValidation(cnhValidationImages)
            .ThenAsync(() => renterRepository.UpdateAsync(renter, cancellationToken));
    }

    private async Task<(Uri front, Uri back)> UploadCNHValidationImagesAsync(
        RenterId renterId,
        IFile frontImage,
        IFile backImage,
        CancellationToken cancellationToken)
    {
        var frontImagePath = RenterStorageUtils.GetFrontCNHImageUrl(renterId, frontImage.Extension);
        var backImagePath = RenterStorageUtils.GetBackCNHImageUrl(renterId, backImage.Extension);

        await Task.WhenAll(
            storageService.UploadAsync(backImagePath, backImage, cancellationToken),
            storageService.UploadAsync(frontImagePath, frontImage, cancellationToken));

        return (frontImagePath, backImagePath);
    }
}