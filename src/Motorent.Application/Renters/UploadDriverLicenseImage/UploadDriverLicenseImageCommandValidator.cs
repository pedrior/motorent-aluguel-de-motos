using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UploadDriverLicenseImage;

internal sealed class UploadDriverLicenseImageCommandValidator 
    : AbstractValidator<UploadDriverLicenseImageCommand>
{
    public UploadDriverLicenseImageCommandValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Must not be null.")
            .Image();
    }
}