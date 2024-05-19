using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UpdateDriverLicense;

internal sealed class UpdateDriverLicenseCommandValidator : AbstractValidator<UpdateDriverLicenseCommand>
{
    public UpdateDriverLicenseCommandValidator()
    {
        RuleFor(x => x.Number)
            .DriverLicenseNumber();

        RuleFor(x => x.Category)
            .DriverLicenseCategory();

        RuleFor(x => x.Expiry)
            .DriverLicenseExpiry();
    }
}