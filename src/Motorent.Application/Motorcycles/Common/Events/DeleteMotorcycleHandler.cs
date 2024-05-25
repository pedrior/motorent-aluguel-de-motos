using Microsoft.Extensions.Logging;
using Motorent.Domain.Motorcycles.Events;
using Motorent.Domain.Motorcycles.Repository;

namespace Motorent.Application.Motorcycles.Common.Events;

internal sealed class DeleteMotorcycleHandler(
    IMotorcycleRepository motorcycleRepository,
    ILogger<DeleteMotorcycleHandler> logger) : IEventHandler<DeleteMotorcycle>
{
    public Task Handle(DeleteMotorcycle e, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling event {EventName} ({@Event})", nameof(DeleteMotorcycle), e);

        return motorcycleRepository.DeleteAsync(e.MotorcycleId, cancellationToken);
    }
}