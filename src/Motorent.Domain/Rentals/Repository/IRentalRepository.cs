using Motorent.Domain.Common.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Rentals.Repository;

public interface IRentalRepository : IRepository<Rental, RentalId>
{
    Task<IReadOnlyList<Rental>> ListRentalsByRenterAsync(
        RenterId renterId,
        int page,
        int limit,
        CancellationToken cancellationToken);
    
    Task<IReadOnlyList<Rental>> ListRentalsByMotorcycleAsync(
        MotorcycleId motorcycleId, CancellationToken cancellationToken);
    
    Task<int> CountRentalsByRenterAsync(RenterId renterId, CancellationToken cancellationToken);
}