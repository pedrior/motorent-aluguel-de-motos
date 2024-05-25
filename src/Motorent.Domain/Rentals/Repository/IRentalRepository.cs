using Motorent.Domain.Common.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.ValueObjects;

namespace Motorent.Domain.Rentals.Repository;

public interface IRentalRepository : IRepository<Rental, RentalId>
{
    Task<IReadOnlyList<Rental>> ListRentalsByMotorcycleAsync(
        MotorcycleId motorcycleId, CancellationToken cancellationToken);
}