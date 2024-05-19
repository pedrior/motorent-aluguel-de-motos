using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Renters;

internal sealed class DocumentService(DataContext dataContext) : IDocumentService
{
    public async Task<bool> IsUniqueAsync(Document document, CancellationToken cancellationToken = default) => 
        !await dataContext.Renters.AnyAsync(r => r.Document == document, cancellationToken);
}