using Motorent.Application.Common.Abstractions.Storage;
using Motorent.Application.Common.Imaging;

namespace Motorent.Application.Common.Validations;

internal static class CommonValidations
{
    public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Length(min: 2, max: 50)
            .WithMessage("Deve ter entre 2 e 50 caracteres.")
            .Matches(@"^(?!.*['.]$)[\p{L}'. }]+$")
            .WithMessage("Deve conter apenas letras e espaços, pontos e apóstrofos seguidos de letras.");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> Birthdate<T>(this IRuleBuilder<T, DateOnly> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Deve estar no passado.");
    }
    
    public static IRuleBuilderOptions<T, string> CNPJ<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Length(11, 18)
            .WithMessage("Deve ter entre 11 e 18 caracteres.")
            .Matches(@"^[\d.\-/]+$")
            .WithMessage("Deve conter apenas números, pontos, barras e hífenes.");
    }
    
    public static IRuleBuilderOptions<T, string> CNHNumber<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Length(11, 11)
            .WithMessage("Deve ter 11 caracteres.")
            .Matches(@"^\d{11}$")
            .WithMessage("Deve conter apenas números.");
    }

    public static IRuleBuilderOptions<T, string> CNHCategory<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .Must(v => Domain.Renters.Enums.CNHCategory.IsDefined(v, ignoreCase: true))
            .WithMessage($"Deve ser um dos seguintes: {string.Join(", ", Domain.Renters.Enums.CNHCategory.List)}");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> CNHExpDate<T>(this IRuleBuilder<T, DateOnly> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Não deve estar vazio.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Deve estar no futuro.");
    }
    
    public static IRuleBuilderOptions<T, IFile> Image<T>(this IRuleBuilder<T, IFile> rule)
    {
        return rule
            .Must(x => x.Stream.IsImage())
            .WithMessage("Deve ser uma imagem PNG ou BMP válida.");
    }
}