using Motorent.Application.Motorcycles.Common.Validations;

namespace Motorent.Application.Motorcycles.RegisterMotorcycle;

internal sealed class RegisterMotorcycleCommandValidator : AbstractValidator<RegisterMotorcycleCommand>
{
    public RegisterMotorcycleCommandValidator()
    {
        RuleFor(x => x.Model)
            .MotorcycleModel();

        RuleFor(x => x.Brand)
            .MotorcycleBrand();

        RuleFor(x => x.DailyPrice)
            .MotorcycleDailyPrice();

        RuleFor(x => x.Year)
            .MotorcycleYear();

        RuleFor(x => x.LicensePlate)
            .MotorcycleLicensePlate();
    }
}