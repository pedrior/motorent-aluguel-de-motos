namespace Motorent.Application.Motorcycles.Common.Messaging;

public sealed record MotorcycleRegisteredMessage(
    Ulid MotorcycleId,
    string Model,
    int Year,
    DateTimeOffset CreatedAt);