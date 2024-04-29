using Motorent.Application.Auth.Common.Validations;

namespace Motorent.Application.Auth.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .Email();

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Must be not empty.");
    }
}