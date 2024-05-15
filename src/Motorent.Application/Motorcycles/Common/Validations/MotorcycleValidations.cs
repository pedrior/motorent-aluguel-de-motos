using Motorent.Domain.Motorcycles.Enums;

namespace Motorent.Application.Motorcycles.Common.Validations;

internal static class MotorcycleValidations
{
    public static IRuleBuilderOptions<T, string> MotorcycleModel<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Length(min: 2, max: 30)
            .WithMessage("Must be between 2 and 30 characters.");
    }

    public static IRuleBuilderOptions<T, string> MotorcycleBrand<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Must(v => Brand.IsDefined(v, ignoreCase: true))
            .WithMessage($"Must be one of the following: {string.Join(", ", Brand.List)}.");
    }

    public static IRuleBuilderOptions<T, decimal> MotorcycleDailyPrice<T>(this IRuleBuilder<T, decimal> rule)
    {
        return rule
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Must be greater than or equal to 0.00");
    }

    public static IRuleBuilderOptions<T, int> MotorcycleYear<T>(this IRuleBuilder<T, int> rule)
    {
        return rule
            .InclusiveBetween(2010, DateTime.UtcNow.Year + 1)
            .WithMessage($"Must be between 2010 and {DateTime.UtcNow.Year + 1}.");
    }

    public static IRuleBuilderOptions<T, string> MotorcycleLicensePlate<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Length(min: 7, max: 8)
            .WithMessage("Must be between 7 and 8 characters.");
    }
}