using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Common.Imaging;

namespace Motorent.Application.Common.Validations;

internal static class CommonValidations
{
    public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .Length(2, 50)
            .WithMessage("Deve ter entre 2 e 50 caracteres.")
            .Matches(@"^(?!.*['.]$)[\p{L}'. }]+$")
            .WithMessage("Deve conter apenas letras e espaços, pontos e apóstrofos seguidos de letras.");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> Birthdate<T>(this IRuleBuilder<T, DateOnly> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Deve estar no passado.");
    }
    
    public static IRuleBuilderOptions<T, string> Document<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .Length(14, 18)
            .WithMessage("Deve ter entre 14 e 18 caracteres.")
            .Matches(@"^(?!.*[./-]{2})(\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}|\d{14})$")
            .WithMessage("Deve ser um CNPJ válido");
    }
    
    public static IRuleBuilderOptions<T, string> DriverLicenseNumber<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .Length(11)
            .WithMessage("Deve ter 11 caracteres.")
            .Matches(@"^\p{N}+")
            .WithMessage("Deve conter apenas números.");
    }

    public static IRuleBuilderOptions<T, string> DriverLicenseCategory<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .Must(v => Domain.Renters.Enums.DriverLicenseCategory.IsDefined(v, ignoreCase: true))
            .WithMessage($"Deve ser um dos seguintes: {string.Join(", ", Domain.Renters.Enums.DriverLicenseCategory.List)}");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> DriverLicenseExpiry<T>(this IRuleBuilder<T, DateOnly> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve ser vazio.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Deve estar no futuro.");
    }
    
    public static IRuleBuilderOptions<T, IFile> Image<T>(this IRuleBuilder<T, IFile> rule)
    {
        return rule
            .Must(x => x.Stream.IsImage())
            .WithMessage("Deve ser uma imagem PNG ou BMP.");
    }
}