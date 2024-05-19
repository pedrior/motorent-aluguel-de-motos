using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.UpdateDriverLicense;

internal sealed class UpdateDriverLicenseCommandHandler(
    IUserContext userContext,
    IRenterRepository renterRepository,
    IDriverLicenseService driverLicenseService) : ICommandHandler<UpdateDriverLicenseCommand>
{
    public async Task<Result<Success>> Handle(UpdateDriverLicenseCommand command, CancellationToken cancellationToken)
    {
        var driverLicense = DriverLicense.Create(
            number: command.Number,
            category: DriverLicenseCategory.FromName(command.Category, ignoreCase: true),
            expiry: command.ExpDate);

        if (driverLicense.IsFailure)
        {
            return driverLicense.Errors;
        }

        var renter = await renterRepository.FindByUserAsync(userContext.UserId, cancellationToken);
        if (renter is null)
        {
            throw new ApplicationException($"Renter not found for user {userContext.UserId}");
        }

        return await renter.ChangeDriverLicenseAsync(driverLicense.Value, driverLicenseService, cancellationToken)
            .ThenAsync(() => renterRepository.UpdateAsync(renter, cancellationToken));
    }
}