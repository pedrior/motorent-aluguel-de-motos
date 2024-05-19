using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Common.Mappings;

internal sealed class CommonMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MotorcycleId, Ulid>()
            .ConstructUsing(src => src.Value);
    }
}