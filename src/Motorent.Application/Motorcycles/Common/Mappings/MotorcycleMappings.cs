using Motorent.Domain.Motorcycles.Enums;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.Common.Mappings;

internal sealed class MotorcycleMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Brand, string>()
            .ConstructUsing(src => src.Name);

        config.NewConfig<Year, int>()
            .ConstructUsing(src => src.Value);

        config.NewConfig<LicensePlate, string>()
            .ConstructUsing(src => src.Value);
    }
}