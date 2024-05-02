namespace Motorent.Application.Motorcycles.GetMotorcycle;

internal sealed class GetMotorcycleQueryValidator : AbstractValidator<GetMotorcycleQuery>
{
    public GetMotorcycleQueryValidator()
    {
        RuleFor(x => x.IdOrLicensePlate)
            .NotEmpty()
            .WithMessage("Must not be empty.");
    }
}