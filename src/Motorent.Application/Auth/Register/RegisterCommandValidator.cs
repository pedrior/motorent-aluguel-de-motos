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

        RuleFor(x => x.Document)
            .Document();

        RuleFor(x => x.DriverLicenseNumber)
            .DriverLicenseNumber();

        RuleFor(x => x.DriverLicenseCategory)
            .DriverLicenseCategory();
        
        RuleFor(x => x.DriverLicenseExpiry)
            .DriverLicenseExpiry();
    }
}