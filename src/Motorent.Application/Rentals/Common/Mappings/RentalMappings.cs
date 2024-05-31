using Motorent.Contracts.Rentals.Responses;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Application.Rentals.Common.Mappings;

internal sealed class RentalMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RentalPlan, string>()
            .ConstructUsing(src => src.Name);

        config.NewConfig<Money, decimal>()
            .ConstructUsing(src => src.Value);

        config.NewConfig<Rental, RentalResponse>()
            .Map(dst => dst.Start, src => src.Period.Start)
            .Map(dst => dst.End, src => src.Period.End)
            .Map(dst => dst.Return, src => src.ReturnDate);
        
        config.NewConfig<Rental, RentalSummaryResponse>()
            .Map(dst => dst.Start, src => src.Period.Start)
            .Map(dst => dst.End, src => src.Period.End)
            .Map(dst => dst.Return, src => src.ReturnDate);
    }
}