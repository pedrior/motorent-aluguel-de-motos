using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Renters;

internal sealed class DriverLicenseService(DataContext dataContext) : IDriverLicenseService
{
    public async Task<bool> IsUniqueAsync(DriverLicense driverLicense, CancellationToken cancellationToken = default) => 
        !await dataContext.Renters.AnyAsync(r => r.DriverLicense.Number == driverLicense.Number, cancellationToken);
}