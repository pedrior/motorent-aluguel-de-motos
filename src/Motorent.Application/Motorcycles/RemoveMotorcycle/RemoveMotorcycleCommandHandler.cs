using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Repository;

namespace Motorent.Application.Motorcycles.RemoveMotorcycle;

internal sealed class RemoveMotorcycleCommandHandler(
    IMotorcycleRepository motorcycleRepository,
    IRentalRepository rentalRepository,
    IMotorcycleDeletionService motorcycleDeletionService) : ICommandHandler<RemoveMotorcycleCommand>
{
    public async Task<Result<Success>> Handle(RemoveMotorcycleCommand command, CancellationToken cancellationToken)
    {
        var motorcycle = await motorcycleRepository.FindAsync(new MotorcycleId(command.Id), cancellationToken);
        if (motorcycle is null)
        {
            return MotorcycleErrors.NotFound;
        }
        
        var rentals = await rentalRepository.ListRentalsByMotorcycleAsync(motorcycle.Id, cancellationToken);
        return await motorcycleDeletionService.Delete(motorcycle, rentals)
            .ThenAsync(() => motorcycleRepository.UpdateAsync(motorcycle, cancellationToken));   
    }
}