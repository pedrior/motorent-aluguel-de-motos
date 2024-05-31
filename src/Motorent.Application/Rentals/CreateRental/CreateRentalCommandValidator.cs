using Motorent.Application.Rentals.Common.Validations;

namespace Motorent.Application.Rentals.CreateRental;

internal sealed class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.Plan)
            .RentalPlan();

        RuleFor(x => x.MotorcycleId)
            .NotEmpty()
            .WithMessage("Não deve ser vazio.");
    }
}