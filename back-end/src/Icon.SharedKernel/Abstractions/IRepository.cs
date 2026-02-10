namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Full data access for an entity — adds write operations (add, update, remove) on top of read-only access.
/// </summary>
public interface IRepository<TEntity, TPrimaryKey> : IReadRepository<TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Adds a new entity to the store.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities at once.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing entity as modified.
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Marks multiple existing entities as modified.
    /// </summary>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Marks an entity for deletion.
    /// </summary>
    void Remove(TEntity entity);

    /// <summary>
    /// Marks multiple entities for deletion.
    /// </summary>
    void RemoveRange(IEnumerable<TEntity> entities);
}
