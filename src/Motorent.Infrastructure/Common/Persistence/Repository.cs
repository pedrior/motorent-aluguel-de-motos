using Microsoft.EntityFrameworkCore;
using Motorent.Domain.Common.Entities;
using Motorent.Domain.Common.Repository;

namespace Motorent.Infrastructure.Common.Persistence;

internal abstract class Repository<TEntity, TId>(DataContext context) : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : notnull
{
    protected DbSet<TEntity> Set => context.Set<TEntity>();

    public async Task<TEntity?> FindAsync(TId id, CancellationToken cancellationToken = default) =>
        await Set.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyCollection<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await Set
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return items.AsReadOnly();
    }

    public Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default) =>
        Set.AnyAsync(e => e.Id.Equals(id), cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Set.Remove(entity);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        return Set.Where(e => e.Id.Equals(id))
            .ExecuteDeleteAsync(cancellationToken);
    }
}