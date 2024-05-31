namespace Motorent.Domain.Common;

public interface IRepository<TEntity, in TId> where TEntity : class, IEntity<TId> where TId : notnull
{
    Task<TEntity?> FindAsync(TId id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}