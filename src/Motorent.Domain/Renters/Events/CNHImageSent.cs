using Motorent.Domain.Common.Events;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Events;

public sealed record CNHImageSent(RenterId RenterId, Uri CNHImageUrl) : IEvent;