using Motorent.Domain.Common.Repository;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.Domain.Motorcycles.Repository;

public interface IMotorcycleRepository : IRepository<Motorcycle, MotorcycleId>
{
    Task<Motorcycle?> FindByLicensePlateAsync(LicensePlate licensePlate, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Motorcycle>> ListAsync(
        int page,
        int limit,
        string? sort = null,
        string? order = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(string? search = null, CancellationToken cancellationToken = default);
}