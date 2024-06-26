namespace Motorent.Application.Motorcycles.Common.Validations;

internal static class MotorcycleValidations
{
    public static IRuleBuilderOptions<T, string> MotorcycleModel<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .Length(2, 30)
            .WithMessage("Deve ter entre 2 e 30 caracteres.");
    }
    
    public static IRuleBuilderOptions<T, int> MotorcycleYear<T>(this IRuleBuilder<T, int> rule)
    {
        return rule
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage($"Deve ser menor ou igual a {DateTime.UtcNow.Year}");
    }

    public static IRuleBuilderOptions<T, string> MotorcycleLicensePlate<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .MaximumLength(8)
            .WithMessage("Deve ter no máximo 8 caracteres.");
    }
}