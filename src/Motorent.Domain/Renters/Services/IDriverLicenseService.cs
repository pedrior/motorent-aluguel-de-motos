using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Services;

public interface IDriverLicenseService
{
    Task<bool> IsUniqueAsync(DriverLicense driverLicense, CancellationToken cancellationToken = default);
}