namespace Motorent.Application.Rentals.UpdateReturnDate;

internal sealed class UpdateReturnDateCommandValidator : AbstractValidator<UpdateReturnDateCommand>
{
    public UpdateReturnDateCommandValidator()
    {
        RuleFor(x => x.RentalId)
            .NotEmpty()
            .WithMessage("Não deve ser vazio.");
        
        RuleFor(x => x.ReturnDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Deve ser maior ou igual à data corrente (UTC).");
    }
}