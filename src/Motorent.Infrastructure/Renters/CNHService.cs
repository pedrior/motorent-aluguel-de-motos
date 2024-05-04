using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;
using Motorent.Infrastructure.Common.Persistence;

namespace Motorent.Infrastructure.Renters;

internal sealed class CNHService(DataContext dataContext) : ICNHService
{
    public async Task<bool> IsUniqueAsync(CNH cnh, CancellationToken cancellationToken = default) => 
        !await dataContext.Renters.AnyAsync(r => r.CNH.Number == cnh.Number, cancellationToken);
}