using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UploadCNHValidationImages;

internal sealed class UploadCNHValidationImagesCommandValidator 
    : AbstractValidator<UploadCNHValidationImagesCommand>
{
    public UploadCNHValidationImagesCommandValidator()
    {
        RuleFor(x => x.FrontImage)
            .NotNull()
            .WithMessage("Must not be null.")
            .Image();
        
        RuleFor(x => x.BackImage)
            .NotNull()
            .WithMessage("Must not be null.")
            .Image();
    }
}