using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles.Errors;

internal static class MotorcycleErrors
{
    public static Error LicensePlateNotUnique(LicensePlate licensePlate) => Error.Conflict(
        "There is already a motorcycle with the same license plate in the system.",
        code: "motorcycle.license_plate_not_unique",
        details: new() { ["license_plate"] = licensePlate.ToString() });
}