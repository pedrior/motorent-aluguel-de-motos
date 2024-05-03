using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Renters.Persistence;

internal sealed class RenterRepository(DataContext context) : Repository<Renter, RenterId>(context), IRenterRepository
{
    public Task<Renter?> FindByUserAsync(string userId, CancellationToken cancellationToken = default) => 
        Set.SingleOrDefaultAsync(r => r.UserId == userId, cancellationToken);
}