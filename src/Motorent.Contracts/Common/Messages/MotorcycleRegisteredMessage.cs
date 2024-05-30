namespace Motorent.Contracts.Common.Messages;

public sealed record MotorcycleRegisteredMessage(
    Ulid MotorcycleId,
    string Model,
    int Year,
    DateTimeOffset CreatedAt);