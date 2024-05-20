namespace Motorent.Application.Auth.Common.Validations;

internal static class AuthValidations
{
    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .EmailAddress()
            .WithMessage("Deve ser um endereço de email válido");
    }

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .MinimumLength(8)
            .WithMessage("Deve ter pelo menos 8 caracteres.")
            .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).*$")
            .WithMessage("Deve conter pelo menos uma letra maiúscula, uma letra minúscula e um número.");
    }
}