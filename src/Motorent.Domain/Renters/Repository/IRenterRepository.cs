using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Repository;

public interface IRenterRepository : IRepository<Renter, RenterId>
{
    Task<Renter?> FindByUserAsync(string userId, CancellationToken cancellationToken = default);
}