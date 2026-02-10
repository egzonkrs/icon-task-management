using Icon.SharedKernel.Specifications;

namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Read-only data access for a given entity. Supports fetching by ID, filtering, counting, and checking existence.
/// </summary>
public interface IReadRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
{
    /// <summary>
    /// Gets a single entity by its primary key, or null if not found.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first match for a specification, or null.
    /// </summary>
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all matches for a specification, projected to the result type.
    /// </summary>
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all matches for a specification as full entities.
    /// </summary>
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts how many entities match the specification.
    /// </summary>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if at least one entity matches the specification.
    /// </summary>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
