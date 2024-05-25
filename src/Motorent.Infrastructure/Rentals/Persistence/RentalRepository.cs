using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Domain.Rentals;
using Motorent.Domain.Rentals.Repository;
using Motorent.Domain.Rentals.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Rentals.Persistence;

internal sealed class RentalRepository(DataContext context)
    : Repository<Rental, RentalId>(context), IRentalRepository
{
    public async Task<IReadOnlyList<Rental>> ListRentalsByMotorcycleAsync(
        MotorcycleId motorcycleId, CancellationToken cancellationToken)
    {
        var rentals = await Set.AsNoTracking()
            .Where(r => r.MotorcycleId == motorcycleId)
            .ToListAsync(cancellationToken);

        return rentals.AsReadOnly();
    }
}