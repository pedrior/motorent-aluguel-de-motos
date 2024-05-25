using Motorent.Domain.Common.Events;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles.Events;

public sealed class DeleteMotorcycle(MotorcycleId motorcycleId) : IEvent
{
    public MotorcycleId MotorcycleId { get; } = motorcycleId;
}