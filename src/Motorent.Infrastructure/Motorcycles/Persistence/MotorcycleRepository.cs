using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Motorcycles;
using Motorent.Domain.Motorcycles.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Motorcycles.Persistence;

internal sealed class MotorcycleRepository(DataContext context)
    : Repository<Motorcycle, MotorcycleId>(context), IMotorcycleRepository
{
    public Task<Motorcycle?> FindByLicensePlateAsync(LicensePlate licensePlate,
        CancellationToken cancellationToken = default)
    {
        return Set.SingleOrDefaultAsync(m => m.LicensePlate == licensePlate, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Motorcycle>> ListAsync(
        int page,
        int limit,
        string? sort = null,
        string? order = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var items = await Set.AsNoTracking()
            .ApplySearchFilter(search)
            .ApplyOrder(sort, order)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return items.AsReadOnly();
    }

    public Task<int> CountAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        return Set.ApplySearchFilter(search)
            .CountAsync(cancellationToken);
    }
}