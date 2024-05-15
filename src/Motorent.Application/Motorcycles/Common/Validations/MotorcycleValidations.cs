using Motorent.Domain.Motorcycles.Enums;

namespace Motorent.Application.Motorcycles.Common.Validations;

internal static class MotorcycleValidations
{
    public static IRuleBuilderOptions<T, string> MotorcycleModel<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Length(min: 2, max: 30)
            .WithMessage("Deve ter entre 2 e 30 caracteres.");
    }

    public static IRuleBuilderOptions<T, string> MotorcycleBrand<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Must(v => Brand.IsDefined(v, ignoreCase: true))
            .WithMessage($"Deve ser um dos seguintes: {string.Join(", ", Brand.List)}.");
    }

    public static IRuleBuilderOptions<T, decimal> MotorcycleDailyPrice<T>(this IRuleBuilder<T, decimal> rule)
    {
        return rule
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Deve ser maior ou igual a 0,00");
    }

    public static IRuleBuilderOptions<T, int> MotorcycleYear<T>(this IRuleBuilder<T, int> rule)
    {
        return rule
            .InclusiveBetween(2010, DateTime.UtcNow.Year + 1)
            .WithMessage($"Deve situar-se entre 2010 e {DateTime.UtcNow.Year + 1}.");
    }

    public static IRuleBuilderOptions<T, string> MotorcycleLicensePlate<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Length(min: 7, max: 8)
            .WithMessage("Deve ter entre 7 e 8 caracteres.");
    }
}