namespace Motorent.Application.Rentals.GetRental;

internal sealed class GetRentalQueryValidator : AbstractValidator<GetRentalQuery>
{
    public GetRentalQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Não deve ser vazio.");
    }
}