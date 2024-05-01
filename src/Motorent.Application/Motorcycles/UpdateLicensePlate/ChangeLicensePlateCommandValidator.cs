using Motorent.Application.Motorcycles.Common.Validations;

namespace Motorent.Application.Motorcycles.UpdateLicensePlate;

internal sealed class ChangeLicensePlateCommandValidator : AbstractValidator<ChangeLicensePlateCommand>
{
    public ChangeLicensePlateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Must not be empty.");

        RuleFor(x => x.LicensePlate)
            .MotorcycleLicensePlate();
    }
}