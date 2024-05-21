using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.Enums;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters;

namespace Motorent.Domain.Rentals.Services;

public interface IRentalFactory
{
    Result<Rental> Create(Renter renter, RentalId id, MotorcycleId motorcycleId, RentalPlan plan);
}