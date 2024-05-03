using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Renters;

internal sealed class CNPJService(DataContext dataContext) : ICNPJService
{
    public async Task<bool> IsUniqueAsync(CNPJ cnpj, CancellationToken cancellationToken = default) => 
        !await dataContext.Renters.AnyAsync(r => r.CNPJ == cnpj, cancellationToken);
}