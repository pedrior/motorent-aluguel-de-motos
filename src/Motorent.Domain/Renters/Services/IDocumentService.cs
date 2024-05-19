using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Services;

public interface IDocumentService
{
    Task<bool> IsUniqueAsync(Document document, CancellationToken cancellationToken = default);
}