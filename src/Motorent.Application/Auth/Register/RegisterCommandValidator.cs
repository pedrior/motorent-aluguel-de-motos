using Motorent.Application.Auth.Common.Validations;
using Motorent.Application.Common.Validations;

namespace Motorent.Application.Auth.Register;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .Email();

        RuleFor(x => x.Password)
            .Password();

        RuleFor(x => x.GivenName)
            .Name();

        RuleFor(x => x.FamilyName)
            .Name();

        RuleFor(x => x.Birthdate)
            .Birthdate();

        RuleFor(x => x.CNPJ)
            .CNPJ();

        RuleFor(x => x.CNHNumber)
            .CNHNumber();

        RuleFor(x => x.CNHCategory)
            .CNHCategory();
        
        RuleFor(x => x.CNHExpDate)
            .CNHExpDate();
    }
}