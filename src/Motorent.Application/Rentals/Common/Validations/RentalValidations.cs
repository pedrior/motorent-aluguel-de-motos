namespace Motorent.Application.Rentals.Common.Validations;

internal static class RentalValidations
{
    public static IRuleBuilderOptions<T, string> RentalPlan<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .Must(v => Domain.Rentals.Enums.RentalPlan.IsDefined(v, ignoreCase: true))
            .WithMessage($"Deve ser um dos seguintes: {string.Join(", ", Domain.Rentals.Enums.RentalPlan.List)}");
    }
}