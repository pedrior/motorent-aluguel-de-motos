using Motorent.Contracts.Renters.Responses;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Renters.Common.Mappings;

internal sealed class RenterMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Document, string>()
            .ConstructUsing(src => src.ToString());
        
        config.NewConfig<EmailAddress, string>()
            .ConstructUsing(src => src.ToString());

        config.NewConfig<FullName, string>()
            .ConstructUsing(src => src.ToString());

        config.NewConfig<Birthdate, DateOnly>()
            .ConstructUsing(src => src.Value);

        config.NewConfig<Renter, RenterProfileResponse>()
            .Map(dest => dest.DriverLicense.Status, src => src.DriverLicenseStatus.Name)
            .Map(dest => dest.DriverLicense.Number, src => src.DriverLicense.Number)
            .Map(dest => dest.DriverLicense.Category, src => src.DriverLicense.Category.Name)
            .Map(dest => dest.DriverLicense.Expiry, src => src.DriverLicense.Expiry)
            .Map(dest => dest.DriverLicense.ImageUrl, src => src.DriverLicenseImageUrl == null 
                ? null 
                : src.DriverLicenseImageUrl.ToString());
    }
}