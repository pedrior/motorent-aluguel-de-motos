using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles.Errors;

internal static class MotorcycleErrors
{
    public static Error LicensePlateNotUnique(LicensePlate licensePlate) => Error.Conflict(
        "Já existe uma moto com a mesma placa no sistema.",
        code: "motorcycle.license_plate_not_unique",
        details: new() { ["license_plate"] = licensePlate.ToString() });
}