using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.UpdateDailyPrice;

internal sealed class UpdateDailyPriceCommandHandler(IMotorcycleRepository motorcycleRepository)
    : ICommandHandler<UpdateDailyPriceCommand>
{
    public async Task<Result<Success>> Handle(UpdateDailyPriceCommand command,
        CancellationToken cancellationToken)
    {
        var dailyPrice = Money.Create(command.DailyPrice);
        if (dailyPrice.IsFailure)
        {
            return dailyPrice.Errors;
        }

        var motorcycle = await motorcycleRepository.FindAsync(new MotorcycleId(command.Id), cancellationToken);
        if (motorcycle is null)
        {
            return MotorcycleErrors.NotFound;
        }
        
        motorcycle.ChangeDailyPrice(dailyPrice.Value);
        await motorcycleRepository.UpdateAsync(motorcycle, cancellationToken);

        return Success.Value;
    }
}