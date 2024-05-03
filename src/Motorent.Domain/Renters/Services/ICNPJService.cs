using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Services;

public interface ICNPJService
{
    Task<bool> IsUniqueAsync(CNPJ cnpj, CancellationToken cancellationToken = default);
}