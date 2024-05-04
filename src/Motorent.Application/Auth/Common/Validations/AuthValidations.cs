namespace Motorent.Application.Auth.Common.Validations;

internal static class AuthValidations
{
    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .EmailAddress()
            .WithMessage("Must be a valid email address.");
    }

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .MinimumLength(8)
            .WithMessage("Must be at least 8 characters long.")
            .Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).*$")
            .WithMessage("Must contain at least one uppercase letter, one lowercase letter and one digit.");
    }
}