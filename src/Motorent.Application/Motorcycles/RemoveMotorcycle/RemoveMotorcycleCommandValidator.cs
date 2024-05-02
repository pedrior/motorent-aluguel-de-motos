namespace Motorent.Application.Motorcycles.RemoveMotorcycle;

internal sealed class RemoveMotorcycleCommandValidator : AbstractValidator<RemoveMotorcycleCommand>
{
    public RemoveMotorcycleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Must not be empty.");
    }
}