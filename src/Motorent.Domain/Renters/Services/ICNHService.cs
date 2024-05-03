using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Services;

public interface ICNHService
{
    Task<bool> IsUniqueAsync(CNH cnh, CancellationToken cancellationToken = default);
}