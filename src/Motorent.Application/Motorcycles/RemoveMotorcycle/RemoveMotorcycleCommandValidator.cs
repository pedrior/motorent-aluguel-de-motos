namespace Motorent.Application.Motorcycles.RemoveMotorcycle;

internal sealed class RemoveMotorcycleCommandValidator : AbstractValidator<RemoveMotorcycleCommand>
{
    public RemoveMotorcycleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Não deve estar vazio.");
    }
}