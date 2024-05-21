using Motorent.Application.Rentals.Common.Validations;

namespace Motorent.Application.Rentals.Rent;

internal sealed class RentCommandValidator : AbstractValidator<RentCommand>
{
    public RentCommandValidator()
    {
        RuleFor(x => x.Plan)
            .RentalPlan();

        RuleFor(x => x.MotorcycleId)
            .NotEmpty()
            .WithMessage("Não deve ser vazio.");
    }
}