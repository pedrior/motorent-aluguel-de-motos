using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Common.Mappings;

internal sealed class CommonMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MotorcycleId, Ulid>()
            .ConstructUsing(src => src.Value);
        
        config.NewConfig<RentalId, Ulid>()
            .ConstructUsing(src => src.Value);
    }
}