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
}