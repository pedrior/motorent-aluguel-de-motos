using Motorent.Application.Motorcycles.Common.Validations;

namespace Motorent.Application.Motorcycles.UpdateDailyPrice;

internal sealed class UpdateDailyPriceCommandValidator : AbstractValidator<UpdateDailyPriceCommand>
{
    public UpdateDailyPriceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Não deve estar vazio.");

        RuleFor(x => x.DailyPrice)
            .MotorcycleDailyPrice();
    }
}