using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Common.Imaging;

namespace Motorent.Application.Common.Validations;

internal static class CommonValidations
{
    public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Length(min: 2, max: 50)
            .WithMessage("Must be between 2 and 50 characters.")
            .Matches(@"^(?!.*['.]$)[\p{L}'. }]+$")
            .WithMessage("Must contain only letters and spaces, periods and apostrophes followed by letters.");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> Birthdate<T>(this IRuleBuilder<T, DateOnly> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Must be in the past.");
    }
    
    public static IRuleBuilderOptions<T, string> CNPJ<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Length(11, 18)
            .WithMessage("Must be between 11 and 18 characters.")
            .Matches(@"^[\d.\-/]+$")
            .WithMessage("Must contain only numbers, periods, slashes and hyphens.");
    }
    
    public static IRuleBuilderOptions<T, string> CNHNumber<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Length(11, 11)
            .WithMessage("Must be 11 characters.")
            .Matches(@"^\d{11}$")
            .WithMessage("Must contain only numbers.");
    }

    public static IRuleBuilderOptions<T, string> CNHCategory<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Must(v => Domain.Renters.Enums.CNHCategory.IsDefined(v, ignoreCase: true))
            .WithMessage($"Must be one of the following: {string.Join(", ", Domain.Renters.Enums.CNHCategory.List)}");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> CNHExpDate<T>(this IRuleBuilder<T, DateOnly> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Must be in the future.");
    }
    
    public static IRuleBuilderOptions<T, IFile> Image<T>(this IRuleBuilder<T, IFile> rule)
    {
        return rule
            .Must(x => x.Stream.IsImage())
            .WithMessage("Must be a valid image (PNG, JPG, JPEG or BMP).");
    }
}