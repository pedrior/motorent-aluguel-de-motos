using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UpdatePersonalInfo;

internal sealed class UpdatePersonalInfoCommandValidator : AbstractValidator<UpdatePersonalInfoCommand>
{
    public UpdatePersonalInfoCommandValidator()
    {
        RuleFor(x => x.GivenName)
            .Name();

        RuleFor(x => x.FamilyName)
            .Name();

        RuleFor(x => x.Birthdate)
            .Birthdate();
    }
}