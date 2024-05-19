using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Renters.Common.Storage;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.UploadCNHImage;

internal sealed class UploadCNHImageCommandHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IStorageService storageService) : ICommandHandler<UploadCNHImageCommand>
{
    public async Task<Result<Success>> Handle(UploadCNHImageCommand command,
        CancellationToken cancellationToken)
    {
        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        var imageUrl = await UploadCNHImageAsync(renter.Id, command.Image, cancellationToken);
        return await renter.SendCNHImage(imageUrl)
            .ThenAsync(() => renterRepository.UpdateAsync(renter, cancellationToken));
    }

    private async Task<Uri> UploadCNHImageAsync(RenterId renterId, IFile image, CancellationToken cancellationToken)
    {
        var imagePath = RenterStorageUtils.GetCNHImagePath(renterId, image.Extension);
        await storageService.UploadAsync(imagePath, image, cancellationToken);

        return imagePath;
    }
}