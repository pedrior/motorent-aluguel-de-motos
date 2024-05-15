namespace Motorent.Application.Motorcycles.Common.Messaging;

public sealed record MotorcycleRegisteredMessage(
    Ulid MotorcycleId,
    string Model,
    string Brand,
    int Year,
    DateTimeOffset CreatedAt);