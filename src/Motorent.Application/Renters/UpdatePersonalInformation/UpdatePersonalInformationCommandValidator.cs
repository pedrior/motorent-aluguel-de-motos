using Motorent.Application.Common.Validations;

namespace Motorent.Application.Renters.UpdatePersonalInformation;

internal sealed class UpdatePersonalInformationCommandValidator : AbstractValidator<UpdatePersonalInformationCommand>
{
    public UpdatePersonalInformationCommandValidator()
    {
        RuleFor(x => x.GivenName)
            .Name();

        RuleFor(x => x.FamilyName)
            .Name();

        RuleFor(x => x.Birthdate)
            .Birthdate();
    }
}