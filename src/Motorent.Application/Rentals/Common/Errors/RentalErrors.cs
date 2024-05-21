using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Application.Rentals.Common.Errors;

internal static class RentalErrors
{
    public static Error MotorcycleNotFound(MotorcycleId motorcycleId) => Error.NotFound(
        "Moto não encontrada.",
        code: "rental.motorcycle_not_found",
        details: new() { ["motorcycle_id"] = motorcycleId.ToString() });
}