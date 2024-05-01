namespace Motorent.Application.Motorcycles.Get;

internal sealed class GetMotorcycleQueryValidator : AbstractValidator<GetMotorcycleQuery>
{
    public GetMotorcycleQueryValidator()
    {
        RuleFor(x => x.IdOrLicensePlate)
            .NotEmpty()
            .WithMessage("Must not be empty.");
    }
}