using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles.Services;

public interface ILicensePlateService
{
    Task<bool> IsUniqueAsync(LicensePlate licensePlate, CancellationToken cancellationToken = default);
}