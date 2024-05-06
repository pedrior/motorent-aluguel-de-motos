using Motorent.Contracts.Renters.Responses;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.Common.Mappings;

internal sealed class RenterMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CNPJ, string>()
            .ConstructUsing(src => src.ToString());
        
        config.NewConfig<EmailAddress, string>()
            .ConstructUsing(src => src.ToString());

        config.NewConfig<FullName, string>()
            .ConstructUsing(src => src.ToString());

        config.NewConfig<Birthdate, DateOnly>()
            .ConstructUsing(src => src.Value);

        config.NewConfig<Renter, RenterProfileResponse>()
            .Map(dest => dest.CNH.Status, src => src.CNHStatus.Name)
            .Map(dest => dest.CNH.Number, src => src.CNH.Number)
            .Map(dest => dest.CNH.Category, src => src.CNH.Category.Name)
            .Map(dest => dest.CNH.ExpirationDate, src => src.CNH.ExpirationDate)
            .Map(dest => dest.CNH.FrontImage, src => src.CNHValidationImages == null 
                ? null 
                : src.CNHValidationImages.FrontImageUrl)
            .Map(dest => dest.CNH.BackImage, src => src.CNHValidationImages == null 
                ? null 
                : src.CNHValidationImages.BackImageUrl);
    }
}