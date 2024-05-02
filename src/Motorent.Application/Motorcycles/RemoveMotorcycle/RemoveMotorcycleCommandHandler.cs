using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Motorcycles.Common.Errors;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Motorcycles.RemoveMotorcycle;

internal sealed class RemoveMotorcycleCommandHandler(IMotorcycleRepository motorcycleRepository) 
    : ICommandHandler<RemoveMotorcycleCommand>
{
    public async Task<Result<Success>> Handle(RemoveMotorcycleCommand command, CancellationToken cancellationToken)
    {
        var motorcycle = await motorcycleRepository.FindAsync(new MotorcycleId(command.Id), cancellationToken);
        if (motorcycle is null)
        {
            return MotorcycleErrors.NotFound;
        }

        await motorcycleRepository.DeleteAsync(motorcycle, cancellationToken);
        return Success.Value;
    }
}