using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UploadCNHImage;

internal sealed class UploadCNHImageCommandValidator 
    : AbstractValidator<UploadCNHImageCommand>
{
    public UploadCNHImageCommandValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Must not be null.")
            .Image();
    }
}