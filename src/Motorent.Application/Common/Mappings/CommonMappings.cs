using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Common.Mappings;

internal sealed class CommonMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MotorcycleId, string>()
            .ConstructUsing(src => src.ToString());
        
        config.NewConfig<RentalId, string>()
            .ConstructUsing(src => src.ToString());
    }
}