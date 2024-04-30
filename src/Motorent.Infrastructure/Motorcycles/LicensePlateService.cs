using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Motorcycles;

internal sealed class LicensePlateService(DataContext dataContext) : ILicensePlateService
{
    public async Task<bool> IsUniqueAsync(LicensePlate licensePlate, CancellationToken cancellationToken = default) =>
        !await dataContext.Motorcycles.AnyAsync(m => m.LicensePlate == licensePlate, cancellationToken);
}